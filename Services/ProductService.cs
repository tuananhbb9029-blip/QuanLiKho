using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.ProductDtos;

namespace QUANLIKHO.Services
{
    public interface IProductService
    {
        Task<ServiceResult<object>> CreateAsync(ProductCreateDto dto);
        Task<ServiceResult<object>> GetPagedAsync(int page = 1, int pageSize = 20);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db) => _db = db;

        public async Task<ServiceResult<object>> CreateAsync(ProductCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ProductName))
                return new(false, 400, "Tên sản phẩm không được để trống.");

            if (!await _db.Categories.AnyAsync(s => s.Categoryid == dto.Categoryid))
                return new(false, 400, "Danh mục không tồn tại.");

            var product = new Product
            {
                Producname = dto.ProductName,
                Price = dto.Price,
                Categoryid = dto.Categoryid,
                Iventory = new Iventory { Quantity = dto.InitialStock }
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            var result = new
            {
                Message = "Thêm sản phẩm thành công!",
                product.Productid,
                dto.ProductName,
                dto.InitialStock
            };

            return new(true, 200, "Thêm sản phẩm thành công!", result);
        }

        public async Task<ServiceResult<object>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = _db.Products.AsNoTracking();
            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.Productid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    s.Productid,
                    s.Producname,
                    s.Price,
                    s.Categoryid,
                    Iventory = s.Iventory != null ? s.Iventory.Quantity : 0
                })
                .ToListAsync();

            var payload = new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = items
            };

            return new(true, 200, "OK", payload);
        }
    }
}