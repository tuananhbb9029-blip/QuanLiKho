using Microsoft.AspNetCore.Mvc;
using QUANLIKHO.Services;

namespace QUANLIKHO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseHistoryController : ControllerBase
    {
        private readonly IPurchaseHistoryService _purchaseHistoryService;

        public PurchaseHistoryController(IPurchaseHistoryService purchaseHistoryService)
            => _purchaseHistoryService = purchaseHistoryService;

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            string? phone = null, int? customerid = null,
            int page = 1, int pageSize = 20)
        {
            var res = await _purchaseHistoryService.GetPagedAsync(phone, customerid, page, pageSize);
            if (!res.Success)
                return StatusCode(res.StatusCode, res.Message);

            return Ok(res.Data);
        }
    }
}