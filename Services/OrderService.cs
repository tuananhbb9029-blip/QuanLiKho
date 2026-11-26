using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.OrderDtos;

namespace QUANLIKHO.Services
{
    public record ServiceResult<T>(bool Success, int StatusCode, string Message, T? Data = default);

    public interface IOrderService
    {
        Task<ServiceResult<object>> CreateOrderAsync(OrderCreateDto dto);
        Task<ServiceResult<object>> GetPagedAsync(int page = 1, int pageSize = 20, int? customerId = null);
    }

    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db) => _db = db;

        public async Task<ServiceResult<object>> CreateOrderAsync(OrderCreateDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return new(false, 400, "Giá trị sản phẩm phải lớn hơn 0.");

            if (dto.Items.Any(s => s.Quantity <= 0))
                return new(false, 400, "Số lượng đặt phải lớn hơn 0.");

            if (!await _db.Customers.AnyAsync(s => s.Customerid == dto.Customerid))
                return new(false, 400, "Không tồn tại khách hàng.");

            var items = dto.Items
                .GroupBy(s => s.Productid)
                .Select(s => new
                {
                    Productid = s.Key,
                    Quantity = s.Sum(g => g.Quantity)
                }).ToList();

            var ids = items.Select(s => s.Productid).ToList();
            var products = await _db.Products
                .Where(s => ids.Contains(s.Productid))
                .ToDictionaryAsync(p => p.Productid);

            var missing = ids.Where(id => !products.ContainsKey(id)).ToList();
            if (missing.Count > 0)
                return new(false, 400, $"Sản phẩm không tồn tại trong hệ thống: {string.Join(", ", missing)}");

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in items)
                {
                    var quantity = item.Quantity;
                    var affected = await _db.Iventories
                        .Where(i => i.Productid == item.Productid && i.Quantity >= quantity)
                        .ExecuteUpdateAsync(s => s.SetProperty(i => i.Quantity, i => i.Quantity - quantity));

                    if (affected == 0)
                    {
                        await transaction.RollbackAsync();
                        return new(false, 409, $"Sản phẩm '{products[item.Productid].Producname}' không đủ số lượng tồn kho.");
                    }
                }

                var order = new Order { Customerid = dto.Customerid };
                foreach (var item in items)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        Productid = item.Productid,
                        Quantity = item.Quantity,
                        UnitPrice = products[item.Productid].Price
                    });
                }

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                foreach (var item in items)
                {
                    _db.Purchasehistories.Add(new Purchasehistory
                    {
                        Customerid = dto.Customerid,
                        Orderid = order.Orderid,
                        Productid = item.Productid,
                        Quantity = item.Quantity,
                        Totalprice = products[item.Productid].Price * item.Quantity
                    });
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var resultData = new
                {
                    Message = "Đặt hàng thành công!",
                    order.Orderid,
                    TotalAmount = items.Sum(i => products[i.Productid].Price * i.Quantity)
                };

                return new(true, 200, "Đặt hàng thành công!", resultData);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new(false, 500, $"Lỗi hệ thống khi đặt hàng: {ex.Message}");
            }
        }

        public async Task<ServiceResult<object>> GetPagedAsync(int page = 1, int pageSize = 20, int? customerId = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = _db.Orders.AsNoTracking();

            if (customerId != null)
                query = query.Where(o => o.Customerid == customerId);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.Orderid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResponseDto
                {
                    Orderid = o.Orderid,
                    Customerid = o.Customerid,
                    Fullname = o.Customer.Fullname,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.OrderDetails.Sum(d => d.Quantity * d.UnitPrice),
                    Details = o.OrderDetails.Select(d => new OrderDetailResponseDto
                    {
                        Productid = d.Productid,
                        Productname = d.Product.Producname,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            var payload = new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Items = items
            };

            return new(true, 200, "OK", payload);
        }
    }
}