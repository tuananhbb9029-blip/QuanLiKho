using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;

namespace QUANLIKHO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseHistoryController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PurchaseHistoryController(AppDbContext db) => _db = db;

        
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            string? phone = null, int? customerid = null,
            int Page = 1, int PageSize = 20)
        {
            if (Page < 1) Page = 1;
            if (PageSize < 1 || PageSize > 100) PageSize = 20;

            var query = _db.Purchasehistories.AsNoTracking();

            if (customerid != null)
                query = query.Where(h => h.Customerid == customerid);

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var p = phone.Trim();
                query = query.Where(h => h.Customer.Phone.Contains(p));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(h => h.Purchasedate)
                .ThenByDescending(h => h.PurchasehistoryId)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
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

            return Ok(new
            {
                CurrentPage = Page,
                PageSize = PageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)PageSize),
                Items = items
            });
        }
    }
}