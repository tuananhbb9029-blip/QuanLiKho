namespace QUANLIKHO.DTO
{
    public class AuthDtos
    {
        public class RegisterDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public int RoleId { get; set; } // 1: Admin, 2: ThuKho, 3: NhanVien
        }

        public class LoginDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
