using Microsoft.EntityFrameworkCore;
using QUANLIKHO.Models;
using static QUANLIKHO.DTO.TheLoai;

namespace QUANLIKHO.Services
{
    public interface IInventoryService
    {
        Task<ServiceResult<object>> NhapKhoAsync(NhapKhoDto model);
        Task<ServiceResult<object>> XuatKhoAsync(XuatKhoDto model);
    }

    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context) => _context = context;

        public async Task<ServiceResult<object>> NhapKhoAsync(NhapKhoDto model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var inventory = await _context.Iventories.FirstOrDefaultAsync(i => i.Productid == model.ProductId);
                if (inventory == null)
                {
                    inventory = new Iventory { Productid = model.ProductId, Quantity = 0 };
                    _context.Iventories.Add(inventory);
                }

                inventory.Quantity += model.SoLuong;

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

                var data = new
                {
                    message = "Nhập kho và ghi thẻ kho thành công!",
                    tonKhoMoi = inventory.Quantity
                };

                return new(true, 200, "Nhập kho và ghi thẻ kho thành công!", data);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new(false, 500, "Lỗi hệ thống: " + ex.Message);
            }
        }

        public async Task<ServiceResult<object>> XuatKhoAsync(XuatKhoDto model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var inventory = await _context.Iventories.FirstOrDefaultAsync(i => i.Productid == model.ProductId);
                if (inventory == null || inventory.Quantity < model.SoLuong)
                {
                    return new(false, 400, "Sản phẩm không tồn tại hoặc không đủ số lượng trong kho!");
                }

                inventory.Quantity -= model.SoLuong;

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

                var data = new
                {
                    message = "Xuất kho và ghi thẻ kho thành công!",
                    tonKhoConLai = inventory.Quantity
                };

                return new(true, 200, "Xuất kho và ghi thẻ kho thành công!", data);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new(false, 500, "Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}