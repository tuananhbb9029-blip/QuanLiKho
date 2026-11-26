using Microsoft.AspNetCore.Mvc;
using QUANLIKHO.Services;
using static QUANLIKHO.DTO.CategoryDtos;

namespace QUANLIKHO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            var res = await _categoryService.CreateAsync(dto);
            if (!res.Success)
                return StatusCode(res.StatusCode, res.Message);

            return Ok(res.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 50)
        {
            var res = await _categoryService.GetAllAsync(page, pageSize);
            if (!res.Success)
                return StatusCode(res.StatusCode, res.Message);

            return Ok(res.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _categoryService.GetByIdAsync(id);
            if (!res.Success)
                return StatusCode(res.StatusCode, res.Message);

            return Ok(res.Data);
        }
    }
}