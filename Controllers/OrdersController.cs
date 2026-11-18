using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;
using System.Reflection.Metadata.Ecma335;
using static QUANLIKHO.DTO.OrderDtos;

namespace QUANLIKHO.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public OrdersController(AppDbContext db) => _db = db;
        [HttpPost]
        public async Task<IActionResult> create([FromBody] OrderCreateDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("gia tri san pham phai lon hon 1");
            if (dto.Items.Any(s => s.Quantity <= 0))
                return BadRequest("ton kho phai lon hon 1");
            if (!await _db.Customers.AnyAsync(s => s.Customerid == dto.Customerid))
                return BadRequest("khong ton tai khach hang");

            var items = dto.Items
                .GroupBy(s=> s.Productid)
                .Select(s=> new
                {
                    Productid = s.Key,
                    Quantity = s.Sum(g=> g.Quantity)
                }).ToList();
            var ids = items.Select(s=> s.Productid).ToList();
            var Products = await _db.Products
                .Where(s => ids.Contains(s.Productid))
                .ToDictionaryAsync(P=> P.Productid);


             var missing = ids.Where(id => !Products.ContainsKey(id)).ToList();
            if (missing.Count > 0)
                return BadRequest($"Sản phẩm không tồn tại trong hệ thống: {string.Join(", ", missing)}");

            await using var tuananh = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in items)
                {
                    var quantity  = item.Quantity;
                    var affected = await _db.Iventories
                        .Where(i => i.Productid == item.Productid && i.Quantity >= quantity)
                        .ExecuteUpdateAsync(s => s.SetProperty(i => i.Quantity, i => i.Quantity - quantity));
                    if (affected == 0)
                    {
                        await tuananh.RollbackAsync();
                        return Conflict($"Sản phẩm '{Products[item.Productid].Producname}' không đủ số lượng tồn kho.");
                    }
                 }
                var order = new Order { Customerid = dto.Customerid };
                foreach (var item in items) {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        Productid = item.Productid,
                        Quantity = item.Quantity,
                        UnitPrice = Products[item.Productid].Price,
                    });
                }
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();


                // 4.3: Ghi nhận lịch sử mua hàng của khách
                foreach (var item in items)
                {
                    _db.Purchasehistories.Add(new Purchasehistory
                    {
                        Customerid = dto.Customerid,
                        Orderid = order.Orderid,
                        Productid = item.Productid,
                        Totalprice = Products[item.Productid].Price * item.Quantity
                    });
                }
                await _db.SaveChangesAsync();

                // Mọi thứ hoàn tất -> Chốt giao dịch thành công!
                await tuananh.CommitAsync();

                // ---------- BƯỚC 5: Trả kết quả ----------
                return Ok(new
                {
                    Message = "Đặt hàng thành công!",
                    order.Orderid,
                    TotalAmount = items.Sum(i => Products[i.Productid].Price * i.Quantity)
                });
            }
            catch (Exception ex)
            {
                // Hỏi: có lỗi bất ngờ thì sao? -> hủy toàn bộ thay đổi
                await tuananh.RollbackAsync();
                return StatusCode(500, $"Lỗi hệ thống khi đặt hàng: {ex.Message}");
            }
        }
        



        
        [HttpGet]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 20, int? customerid = null)
        {        
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

 
            var query = _db.Orders.AsNoTracking();

            if (customerid != null)
                query = query.Where(o => o.Customerid == customerid);

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

           
            return Ok(new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                Items = items
            });
        }
    }
}