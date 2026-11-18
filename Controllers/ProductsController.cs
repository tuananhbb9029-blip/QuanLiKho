using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLIKHO.DTO;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.ProductDtos;
namespace QUANLIKHO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _DB;
        public ProductsController(AppDbContext db) => _DB = db;
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            // kiểm tra
            if (string.IsNullOrWhiteSpace(dto.ProductName))
                return BadRequest("Tên sản phẩm không được để trống.");
            if (!await _DB.Categories.AnyAsync(s => s.Categoryid == dto.Categoryid))
                return BadRequest("Danh mục không tồn tại.");
             //ánh xạ từ  dto qua model 
            var product = new Product
            {
                Producname = dto.ProductName,
                Price = dto.Price,
                Categoryid = dto.Categoryid,
                Iventory = new Iventory { Quantity = dto.InitialStock }
            };

            _DB.Products.Add(product);
            await _DB.SaveChangesAsync();

            return Ok(new
            {
                Message = "Thêm sản phẩm thành công!",
                product.Productid,
                dto.ProductName,
                dto.InitialStock
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedProducts(int Page = 1,int PageSize = 20)
        {
            if (Page < 1) Page = 1;
            if (PageSize < 1 || PageSize > 100) PageSize = 20;
          //XEM CAI HANG CHO khi nào càn thực thi  thì mới cần ví dụ thực thi luư ,tính toán
            var query =  _DB.Products.AsNoTracking();
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.Productid)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .Select(s => new
                {
                    s.Productid,
                    s.Producname,
                    s.Price,
                    s.Categoryid,
                    Iventory = s.Iventory != null ? s.Iventory.Quantity : 0
                }).ToListAsync();


            return Ok(new
            {
                CurrentPage = Page,
                PageSize = PageSize,
                TotalCount = totalCount,
                TotalPages = (int) Math.Ceiling(totalCount /(double)PageSize),
                Items = items
            });
        }

    }
}