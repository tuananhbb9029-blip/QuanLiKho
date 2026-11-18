namespace QUANLIKHO.DTO
{
    public class TheLoai
    {
        public class NhapKhoDto
        {
            public int ProductId { get; set; }
            public int SoLuong { get; set; }
            public string MaPhieuNhap { get; set; } = string.Empty;
            public int UserId { get; set; }
        }

        public class XuatKhoDto
        {
            public int ProductId { get; set; }
            public int SoLuong { get; set; }
            public string MaChungTu { get; set; } = string.Empty;
            public int UserId { get; set; }
        }
    }
}
