using Microsoft.AspNetCore.Mvc;
using QUANLIKHO.Services;
using static QUANLIKHO.DTO.OrderDtos;

namespace QUANLIKHO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService) => _orderService = orderService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            var res = await _orderService.CreateOrderAsync(dto);
            if (!res.Success)
                return StatusCode(res.StatusCode, res.Message);

            return Ok(res.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 20, int? customerid = null)
        {
            var res = await _orderService.GetPagedAsync(page, pageSize, customerid);
            if (!res.Success)
                return StatusCode(res.StatusCode, res.Message);

            return Ok(res.Data);
        }
    }
}