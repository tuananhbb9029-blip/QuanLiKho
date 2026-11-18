using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.TheLoai;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public InventoryController(AppDbContext context)
    {
        _context = context;
    }

    // 1. API NHẬP KHO
    [HttpPost("nhap-kho")]
    [Authorize(Roles = "Admin,ThuKho")]
    public async Task<IActionResult> NhapKho([FromBody] NhapKhoDto model)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Kiểm tra xem sản phẩm đã có trong bảng tồn kho (Iventory) chưa
            var inventory = await _context.Iventories.FirstOrDefaultAsync(i => i.Productid == model.ProductId);
            if (inventory == null)
            {
                inventory = new Iventory { Productid = model.ProductId, Quantity = 0 };
                _context.Iventories.Add(inventory);
            }

            // Cộng thêm số lượng nhập
            inventory.Quantity += model.SoLuong;

            // Tự động ghi nhận vào bảng Thẻ kho
            var theKho = new TheKho
            {
                Productid = model.ProductId,
                LoaiGiaoDich = "NHAP",
                MaChungTu = model.MaPhieuNhap,
                SoLuongThayDoi = model.SoLuong,
                TonKhoSauKhiDoi = inventory.Quantity,
                ThoiGian = DateTime.Now,
                UserId = model.UserId
            };
            _context.TheKhos.Add(theKho);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Nhập kho và ghi thẻ kho thành công!", tonKhoMoi = inventory.Quantity });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    // 2. API XUẤT KHO
    [HttpPost("xuat-kho")]
    [Authorize(Roles = "Admin,ThuKho")]
    public async Task<IActionResult> XuatKho([FromBody] XuatKhoDto model)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Kiểm tra sản phẩm có trong kho và có đủ số lượng để xuất không
            var inventory = await _context.Iventories.FirstOrDefaultAsync(i => i.Productid == model.ProductId);
            if (inventory == null || inventory.Quantity < model.SoLuong)
            {
                return BadRequest(new { message = "Sản phẩm không tồn tại hoặc không đủ số lượng trong kho!" });
            }

            // Trừ số lượng tồn kho
            inventory.Quantity -= model.SoLuong;

            // Tự động ghi nhận vào bảng Thẻ kho
            var theKho = new TheKho
            {
                Productid = model.ProductId,
                LoaiGiaoDich = "XUAT",
                MaChungTu = model.MaChungTu,
                SoLuongThayDoi = model.SoLuong,
                TonKhoSauKhiDoi = inventory.Quantity,
                ThoiGian = DateTime.Now,
                UserId = model.UserId
            };
            _context.TheKhos.Add(theKho);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Xuất kho và ghi thẻ kho thành công!", tonKhoConLai = inventory.Quantity });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}