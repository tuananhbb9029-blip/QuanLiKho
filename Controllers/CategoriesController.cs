using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLIKHO.DTO;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.CategoryDtos;
namespace QUANLIKHO.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db) => _db = db;
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Categoryname))
                return BadRequest("khong ton tai loai san pham");
            var Category = new Category
            {
                Categoryname = dto.Categoryname
            };
            _db.Categories.Add(Category);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "them thanh cong roi nha",
                Category.Categoryid,
                dto.Categoryname
            });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int Page = 1, int PageSize = 50)
        {
            if (Page < 1) Page = 1;
            if (PageSize < 1 || PageSize > 200) PageSize = 50;

            var query = _db.Categories.AsNoTracking();
            var total = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Categoryid)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new
                {
                    c.Categoryid,
                    c.Categoryname,
                    ProductCount = c.Products.Count()
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