using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUANLIKHO.Services;
using static QUANLIKHO.DTO.TheLoai;

namespace QUANLIKHO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService) => _inventoryService = inventoryService;

        [HttpPost("nhap-kho")]
        [Authorize(Roles = "Admin,ThuKho")]
        public async Task<IActionResult> NhapKho([FromBody] NhapKhoDto model)
        {
            var res = await _inventoryService.NhapKhoAsync(model);
            if (!res.Success)
                return StatusCode(res.StatusCode, new { message = res.Message });

            return Ok(res.Data);
        }

        [HttpPost("xuat-kho")]
        [Authorize(Roles = "Admin,ThuKho")]
        public async Task<IActionResult> XuatKho([FromBody] XuatKhoDto model)
        {
            var res = await _inventoryService.XuatKhoAsync(model);
            if (!res.Success)
                return StatusCode(res.StatusCode, new { message = res.Message });

            return Ok(res.Data);
        }
    }
}