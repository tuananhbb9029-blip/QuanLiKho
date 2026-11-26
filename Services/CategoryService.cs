using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.CategoryDtos;

namespace QUANLIKHO.Services
{
    public interface ICategoryService
    {
        Task<ServiceResult<object>> CreateAsync(CategoryCreateDto dto);
        Task<ServiceResult<object>> GetAllAsync(int page = 1, int pageSize = 50);
        Task<ServiceResult<object>> GetByIdAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;

        public CategoryService(AppDbContext db) => _db = db;

        public async Task<ServiceResult<object>> CreateAsync(CategoryCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Categoryname))
                return new(false, 400, "Không tồn tại loại sản phẩm");

            var category = new Category
            {
                Categoryname = dto.Categoryname
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            var data = new
            {
                Message = "Thêm thành công rồi nha",
                category.Categoryid,
                dto.Categoryname
            };

            return new(true, 200, "Thêm thành công rồi nha", data);
        }

        public async Task<ServiceResult<object>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 200) pageSize = 50;

            var query = _db.Categories.AsNoTracking();
            var total = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Categoryid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Categoryid,
                    c.Categoryname,
                    ProductCount = c.Products.Count()
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

        public async Task<ServiceResult<object>> GetByIdAsync(int id)
        {
            var category = await _db.Categories
                .AsNoTracking()
                .Where(s => s.Categoryid == id)
                .Select(s => new CategoryResponseDto
                {
                    Categoryid = s.Categoryid,
                    Categoryname = s.Categoryname,
                    TotalProducts = s.Products.Count()
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return new(false, 404, "Sản phẩm không tồn tại !!!");

            return new(true, 200, "OK", category);
        }
    }
}