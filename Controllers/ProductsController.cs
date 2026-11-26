using Microsoft.AspNetCore.Mvc;
using QUANLIKHO.Services;
using static QUANLIKHO.DTO.ProductDtos;

namespace QUANLIKHO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService) => _productService = productService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var res = await _productService.CreateAsync(dto);
            if (!res.Success)
                return StatusCode(res.StatusCode, res.Message);

            return Ok(res.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedProducts(int page = 1, int pageSize = 20)
        {
            var res = await _productService.GetPagedAsync(page, pageSize);
            if (!res.Success)
                return StatusCode(res.StatusCode, res.Message);

            return Ok(res.Data);
        }
    }
}