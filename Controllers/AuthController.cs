using Microsoft.AspNetCore.Mvc;
using QUANLIKHO.Services;
using static QUANLIKHO.DTO.AuthDtos;

namespace QUANLIKHO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var res = await _authService.RegisterAsync(model);
            if (!res.Success)
                return StatusCode(res.StatusCode, new { message = res.Message });

            return Ok(res.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var res = await _authService.LoginAsync(model);
            if (!res.Success)
                return StatusCode(res.StatusCode, new { message = res.Message });

            return Ok(res.Data);
        }
    }
}