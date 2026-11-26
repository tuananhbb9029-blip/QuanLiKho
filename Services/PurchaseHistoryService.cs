using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;

namespace QUANLIKHO.Services
{
    public interface IPurchaseHistoryService
    {
        Task<ServiceResult<object>> GetPagedAsync(string? phone = null, int? customerId = null, int page = 1, int pageSize = 20);
    }

    public class PurchaseHistoryService : IPurchaseHistoryService
    {
        private readonly AppDbContext _db;

        public PurchaseHistoryService(AppDbContext db) => _db = db;

        public async Task<ServiceResult<object>> GetPagedAsync(string? phone = null, int? customerId = null, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = _db.Purchasehistories.AsNoTracking();

            if (customerId != null)
                query = query.Where(h => h.Customerid == customerId);

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var p = phone.Trim();
                query = query.Where(h => h.Customer.Phone.Contains(p));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(h => h.Purchasedate)
                .ThenByDescending(h => h.PurchasehistoryId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new
                {
                    h.PurchasehistoryId,
                    h.Customerid,
                    Customername = h.Customer.Fullname,
                    h.Customer.Phone,
                    h.Orderid,
                    h.Productid,
                    h.Product.Producname,
                    h.Quantity,
                    h.Totalprice,
                    h.Purchasedate
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