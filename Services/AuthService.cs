using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.AuthDtos;

namespace QUANLIKHO.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<object>> RegisterAsync(RegisterDto model);
        Task<ServiceResult<object>> LoginAsync(LoginDto model);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context) => _context = context;

        public async Task<ServiceResult<object>> RegisterAsync(RegisterDto model)
        {
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return new(false, 400, "Tên đăng nhập đã tồn tại!");
            }

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

            return new(true, 200, "Đăng ký thành công!", new { message = "Đăng ký thành công!" });
        }

        public async Task<ServiceResult<object>> LoginAsync(LoginDto model)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
            {
                return new(false, 401, "Sai tên đăng nhập hoặc mật khẩu!");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return new(false, 401, "Sai tên đăng nhập hoặc mật khẩu!");
            }

            var data = new
            {
                message = "Đăng nhập thành công!",
                userId = user.UserId,
                username = user.Username,
                fullName = user.FullName,
                role = user.Role?.RoleName
            };

            return new(true, 200, "Đăng nhập thành công!", data);
        }
    }
}