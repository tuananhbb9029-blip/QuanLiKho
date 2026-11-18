using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLIKHO.DTO;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.AuthDtos;

namespace QUANLIKHO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context) => _context = context;
        

        // 1. API ĐĂNG KÝ
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // Kiểm tra xem username đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });
            }

            // Mã hóa mật khẩu bằng BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                PasswordHash = passwordHash,
                FullName = model.FullName,
                RoleId = model.RoleId,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!" });
        }

        // 2. API ĐĂNG NHẬP
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _context.Users
                .Include(u => u.Role) // Lấy kèm thông tin bảng Role để biết quyền gì
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu!" });
            }

            // Kiểm tra mật khẩu đã mã hóa
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu!" });
            }

            // Đăng nhập thành công (Tạm thời trả về thông tin user và tên quyền)
            return Ok(new
            {
                message = "Đăng nhập thành công!",
                userId = user.UserId,
                username = user.Username,
                fullName = user.FullName,
                role = user.Role?.RoleName // Ví dụ: Admin, ThuKho, NhanVien
            });
        }

    }
}