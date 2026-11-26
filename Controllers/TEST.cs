using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Identity.Client;
using QUANLIKHO.Models;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Net.WebSockets;
using static QUANLIKHO.DTO.OrderDtos;


namespace QUANLIKHO.Controllers
{
    public class ProductDTO
    {
        public string Producname { get; set; } = "";
        public decimal Price { get; set; }

    }
    public class PagingRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class page<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
    [ApiController]
    [Route("api/[Controller]")]
    public class TEST : ControllerBase
    {
        private readonly AppDbContext _DB;
        public TEST(AppDbContext db) => _DB = db;

        [HttpGet]
        public async Task<IActionResult> getProduct()
        {
            var Product = await _DB.Products.Where(p => p.Price < 5000).ToListAsync();
            if (Product.Count == 0)
            {
                return NotFound("khong tim thay");
            }
            return Ok(Product);
        }
        [HttpPost]
        public async Task<IActionResult> createProduct([FromBody] ProductDTO Productdto)
        {
            if (Productdto.Price < 0)
            {
                return NotFound("du lieu khong hop le");
            }
            var Product = new Product
            {
                Producname = Productdto.Producname,
                Price = Productdto.Price

            };
            _DB.Products.Add(Product);
            await _DB.SaveChangesAsync();

            return Ok(Product);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string name)
        {
            // Bước 1: Kiểm tra từ khóa
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Vui lòng nhập tên sản phẩm.");
            }

            // Bước 2: Tìm sản phẩm theo tên
            var products = await _DB.Products
                .Where(p => p.Producname.Contains(name))
                .ToListAsync();


            var gia = await _DB.Products
                .AsNoTracking()
                .Where(s => s.Price >= 5000 && s.Price <= 200000)
                .ToListAsync();

            var tensp = await _DB.Products
                .AsNoTracking()
                .Where(s => s.Producname.Contains("SAN PHAM"))
                .ToListAsync();

            var DATNHAT = await _DB.Products
                .AsNoTracking()
                .OrderByDescending(s => s.Price)
                .Take(10)
                .Select(s => new
                {
                    s.Productid,
                    s.Producname,
                    s.Price
                }).ToListAsync();

            var ten = await _DB.Customers
                .AsNoTracking()
                .Where(s => s.Fullname.StartsWith("N"))
                .Select(s => new
                {
                    s.Customerid,
                    s.Fullname
                }).ToListAsync();



            var cacdonkhach = await _DB.Orders
                .AsNoTracking()
                .OrderByDescending(s => s.Orderid)
                .Where(s => s.Customerid == 7)
                .ToArrayAsync();



            var loaidanhmuc = await _DB.Products
                .AsNoTracking()
                .Where(s => s.Categoryid == 2 || s.Categoryid == 4)
                .ToArrayAsync();

            var tonkho = await _DB.Iventories
                .AsNoTracking()
                .Where(s => s.Quantity > 500)
                .Select(s => new
                {
                    s.Productid,
                    s.Product.Producname
                })
                .ToArrayAsync();


            var sapsep = await _DB.Customers
                .AsNoTracking()
                .OrderBy(S => S.Fullname)
                .Take(5).ToListAsync();


            var products1 = await _DB.Products
                .AsNoTracking()
                .Where(s => s.Price > 10000).ToListAsync();

            var Producname = await _DB.Products
                .AsNoTracking()
                .OrderBy(s => s.Price)
                .Take(10).Select(p => p.Producname).ToListAsync();


            var demsanpham = await _DB.Products
                .AsNoTracking()
                .CountAsync(s => s.Categoryid == 1);

            var sodt = await _DB.Customers
                .AsNoTracking()
                .Where(s => s.Phone.StartsWith("090")).AnyAsync();

            var sanpham = await _DB.Iventories
                .AsNoTracking()
                .Where(s => s.Quantity == 0)
                .Select(s => s.Product)
                .ToListAsync();

            var donhang = await _DB.Orders
                .AsNoTracking()
                .OrderByDescending(s => s.OrderDate)
                .Take(5).ToListAsync();


            var dem1 = await _DB.Customers
                .AsNoTracking()
                .CountAsync();

            var dem2 = await _DB.Orders
                .AsNoTracking()
                .Where(s => s.Customerid == 7)
                .AnyAsync();

            var dem3 = await _DB.Orders
                .AsNoTracking()
                .Where(s => s.Customerid == 7)
                .CountAsync();


            var tongkho = await _DB.Iventories
               .AsNoTracking()
               .SumAsync(s => s.Quantity);

            var giacao = await _DB.Products
                .AsNoTracking()
                .Where(s => s.Categoryid == 1)
                .MaxAsync(s => s.Price);


            var tb = await _DB.Products
                .AsNoTracking()
                .Where(s => s.Categoryid == 3)
                .AverageAsync(s => s.Price);


            var moinhat = await _DB.Orders
               .AsNoTracking()
               .OrderByDescending(S => S.Orderid)
               .FirstOrDefaultAsync();

            var sp = await _DB.Products
                .AsNoTracking()
                .Select(s => new
                {
                    s.Productid,
                    s.Producname,
                    Categoryname = s.Category.Categoryname
                }).ToListAsync();

            // toan tu kha hay 
            var lay = await _DB.Products
                .AsNoTracking()
                .Select(s => new
                {
                    s.Productid,
                    s.Producname,
                    tonkho = s.Iventory != null ? s.Iventory.Quantity : 0
                }).ToListAsync();

            var ds = _DB.Orders
              .AsNoTracking()
              .Select(s => new
              {
                  s.Orderid,
                  s.OrderDate,
                  ten = s.Customer.Fullname
              }).ToListAsync();


            var co1don = await _DB.Customers
                .AsNoTracking()
                .Where(s => s.Orders.Any())
                .Select(c => new { c.Customerid, c.Fullname, c.Phone })
                 .ToListAsync();

            var dcban = await _DB.Products
                .AsNoTracking()
                .Where(s => s.OrderDetails.Any())
                .Select(s => new
                {
                    s.Productid,
                    s.Producname,

                }).ToListAsync();


            var lay1 = await _DB.Products
               .AsNoTracking()
               .Select(s => new
               {
                   s.Productid,
                   s.Producname,
                   s.Category.Categoryname
               }).ToListAsync();


            var lay2 = await _DB.OrderDetails
                .AsNoTracking()
                .Select(s => new
                {
                    s.Orderid,
                    s.Order.Customer.Fullname,
                    s.Product.Producname,
                    s.Quantity,
                    thanhtien = s.UnitPrice * s.Quantity
                }).ToListAsync();


            var lay3 = await _DB.Orders
                .AsNoTracking()
                .Select(s => new
                {
                    s.Orderid,
                    tongtien = s.OrderDetails.Sum(s1 => s1.Quantity * s1.UnitPrice)
                }).ToListAsync();


            var lay4 = await _DB.Products
                .AsNoTracking()
                .Select(s => new
                {
                    s.Productid,
                    s.Producname,
                    daban = s.OrderDetails.Sum(s1 => s1.Quantity)
                }).ToListAsync();

            var lay5 = await _DB.Products
                .AsNoTracking()
                .OrderBy(s => s.Price)
                .Select(s => new
                {
                    s.Productid,
                    s.Producname,
                    s.Price,
                    phanloai = s.Price < 1000000 ? "re" :
                    (s.Price < 10000000 ? "trung binh" : "cao cap")
                }).ToListAsync();

            var lay6 = await _DB.OrderDetails
                .AsNoTracking()
                .GroupBy(s => new
                {
                    nam = s.Order.OrderDate.Year,
                    thang = s.Order.OrderDate.Month
                }).Select(s => new
                {
                    nam = s.Key.nam,
                    thang = s.Key.thang,
                    tongtien = s.Sum(d => d.Quantity * d.UnitPrice)
                })
                .OrderBy(s => s.nam)
                .ThenBy(s => s.thang)
                .ToListAsync();

            //--------------------------------------------------------

            var yeucau = await _DB.Categories
                .AsNoTracking()
                .Select(s => new
                {
                    s.Categoryid,
                    s.Categoryname,
                    s.Products.Count,
                }).ToListAsync();


            var yeucau1 = await _DB.Products
                .AsNoTracking()
                .Select(s => new
                {
                    s.Categoryid,
                    s.Category.Categoryname,
                    tong = s.OrderDetails.Sum(a => a.Quantity)
                }).ToListAsync();

            var yeucau2 = await _DB.Orders
                .Select(s => new
                {
                    s.Orderid,
                    tongtien = s.OrderDetails.Sum(a => a.Quantity)
                }).ToListAsync();


            var yeucau3 = await _DB.OrderDetails
                .AsNoTracking()
                .GroupBy(s => new
                {
                    s.Productid,
                    s.Product.Producname
                }).Select(g => new
                {
                    g.Key.Productid,
                    tong = g.Sum(x => x.Quantity)
                }).ToListAsync();


            var yeucau4 = await _DB.Orders
                .AsNoTracking()
                .GroupBy(s => new
                {
                    s.Customerid,
                    s.Customer.Fullname
                })
                .Select(g => new
                {
                    g.Key.Customerid,
                    g.Key.Fullname,
                    tong = g.Sum(d => d.OrderDetails.Sum(m => m.Quantity * m.UnitPrice))
                }).ToListAsync();


            var yeucau5 = await _DB.Customers
                .AsNoTracking()
                .Select(s => new
                {
                    s.Customerid,
                    s.Fullname,
                    soluong = s.Orders.Count()
                }).ToListAsync();

            var yeucau6 = await _DB.Orders
                .AsNoTracking()
                .Where(s => s.OrderDate != null)
                .GroupBy(s => new
                {
                    thang = s.OrderDate.Month,
                    nam = s.OrderDate.Year
                })
                .Select(g => new
                {
                    g.Key.thang,
                    g.Key.nam,
                    tongdongthu = g.Sum(l => l.OrderDetails.Sum(m => m.Quantity * m.UnitPrice))
                }).ToListAsync();


            string keyword = "a";
            int? categoryId = 1;
            decimal? minPrice = 1000000;
            decimal? maxPrice = 5000000;

            var yeucau7 = await _DB.Products
              .AsNoTracking()
              .Where(s =>
              (string.IsNullOrEmpty(keyword) || s.Producname.Contains(keyword))
              && (!categoryId.HasValue || s.Categoryid == categoryId.Value)
              && (!minPrice.HasValue || s.Price >= minPrice)
              && (!maxPrice.HasValue || s.Price <= maxPrice)
              ).ToListAsync();


            var yeucau8 = await _DB.Customers
                .AsNoTracking()
                .Where(s => s.Orders.Sum(g => g.OrderDetails.Sum(d => d.Quantity * d.UnitPrice)) > 1000000000)
                .Select(s => new
                {
                    s.Customerid,
                    s.Fullname,
                    tong = s.Orders.Sum(g => g.OrderDetails.Sum(d => d.Quantity * d.UnitPrice))
                }).ToListAsync();

            var yeucau9 = await _DB.OrderDetails
                .AsNoTracking()
                .GroupBy(s => new
                {
                    s.Productid,
                    s.Product.Producname
                })
                .Select(s => new
                {
                    s.Key.Productid,
                    s.Key.Producname,
                    tongsl = s.Sum(d => d.Quantity)
                }).OrderByDescending(x => x.tongsl)
                .FirstOrDefaultAsync();

            int PageNumber = 2;
            int PageSize = 10;

            var yeucau10 = await _DB.Products
                .AsNoTracking()
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();


            var yeucau11 = await _DB.Products
                .AsNoTracking()
                .OrderBy(s => s.Price)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var yeucau12 = await _DB.Orders
               .AsNoTracking()
               .OrderByDescending(s => s.OrderDate)
               .Skip((PageNumber - 1) * PageSize)
               .Take(PageSize)
               .ToListAsync();

            int customerId = 5;
            var yeucau13 = await _DB.Orders
                .AsNoTracking()
                .Where(s => s.Customerid == customerId)
                .OrderByDescending(s => s.OrderDate)
                .Skip((PageNumber - 1) * PageSize)
               .Take(PageSize)
               .ToListAsync();

            var datNhat = await _DB.Products
              .AsNoTracking()
              .OrderByDescending(p => p.Price)
               .FirstOrDefaultAsync();

            var reNhat = await _DB.Products
                .AsNoTracking()
                .OrderBy(p => p.Price)
                .FirstOrDefaultAsync();

            var trungbinh = await _DB.Products
                .AsNoTracking()
                .AverageAsync(s => s.Price);


            var sanpham2 = await _DB.Products
                .AsNoTracking()
                .GroupBy(s => s.Categoryid)
                .Select(g => new
                {
                    Categoryid = g.Key,
                    productscount = g.Count(),
                }).ToListAsync();

            var tonkho1 = await _DB.Iventories
                .AsNoTracking()
                .GroupBy(s => s.Product.Categoryid)
                .Select(g => new
                {
                    Categoryid = g.Key,
                    tonkho = g.Sum(i => i.Quantity)
                }).ToListAsync();

            var tongtien1 = await _DB.OrderDetails
                .AsNoTracking()
                .GroupBy(s => s.Orderid)
                .Select(g => new
                {
                    Orderid = g.Key,
                    tongtungdon = g.Sum(d => d.Quantity * d.UnitPrice)
                }).ToListAsync();

            var dadat = await _DB.Customers
               .AsNoTracking()
            .Select(c => new
            {
                c.Customerid,
                c.Fullname,
                SoDon = c.Orders.Count()
            })
             .ToListAsync();

            var dt = await _DB.OrderDetails
                .AsNoTracking()
                .Where(s => s.Order.Customerid == 5)
                .Select(s => new
                {
                    s.Productid,
                    s.Product.Producname
                }).Distinct().ToListAsync();


            int page = 2;
            int pageSize = 15;

            var phantrang = await _DB.Products
                .AsNoTracking()
                .OrderBy(s => s.Price)
                .ThenBy(s => s.Productid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Productid,
                    p.Producname,
                    p.Price
                }).ToListAsync();


            // Bước 3: Kiểm tra kết quả
            if (products.Count == 0)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            // Bước 4: Trả kết quả
            return Ok(products);

        }

        [HttpGet("search")]
        public async Task<IActionResult> searchproducts
        (
            string? keyword,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int page = 1,
            int pageSize = 10
        )
        {
            var query = _DB.Products
                .AsNoTracking()
                .Where(s =>
                string.IsNullOrEmpty(keyword) || s.Producname.Contains(keyword)
                && (!categoryId.HasValue || s.Categoryid == categoryId.Value)
                && (!minPrice.HasValue || s.Price >= minPrice.Value)
                && (!maxPrice.HasValue || s.Price <= maxPrice.Value)

                );
            query = sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(s => s.Price),
                "price_desc" => query.OrderByDescending(s => s.Price),
                "name_asc" => query.OrderBy(s => s.Producname),
                "name_desc" => query.OrderByDescending(s => s.Producname),
                _ => query.OrderBy(s => s.Productid)
            };

            int totalItems = await query.CountAsync();    // đếm bản ghi thỏa mãn điêu kiện 


            var phantrang = await query
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems
                / (double)pageSize),
                data = phantrang
            });
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            int totalProducts = await _DB.Products.CountAsync();
            int totalCustomers = await _DB.Customers.CountAsync();
            int totalOrders =  await _DB.Orders.CountAsync();
            // Tồn kho: cộng cột Quantity của bảng Iventory
            var totalStock = await _DB.Iventories.SumAsync(i => i.Quantity);

            // Doanh thu: số lượng bán x giá sản phẩm
            var totalRevenue = await _DB.OrderDetails
                .SumAsync(d => d.Quantity * d.Product.Price);


            var SANPHAMBANCHAY = await _DB.OrderDetails
                .AsNoTracking()
                .GroupBy(s => new
                {
                    s.Productid,
                    s.Product.Producname
                })
                .Select(s => new
                {
                    s.Key.Productid,
                    s.Key.Producname,
                    tong = s.Sum(g => g.Quantity * g.UnitPrice)
                })
                .OrderByDescending(s=> s.tong)
                .Take(5)
                .ToListAsync();


            return Ok(new
            {
                TotalProducts = totalProducts,
                TotalCustomers = totalCustomers,
                TotalOrders = totalOrders,
                TotalStock = totalStock,
                TotalRevenue = totalRevenue,
                TopSellingProducts = SANPHAMBANCHAY
            });
        }
        [HttpGet("customer-stats")]
        public async Task<IActionResult> geGetCustomerStats()
        {
            var customerStats = await _DB.Customers
                .Select(s => new
                {
                    CustomerId = s.Customerid,
                    Fullname = s.Fullname,
                    TotalOrders = s.Orders.Count(),
                    TotalProductsBought = s.Orders.Sum(k => k.OrderDetails.Sum(d => (int?)d.Quantity)) ?? 0,
                    TotalMoneySpent = s.Orders.Sum(k => k.OrderDetails.Sum(d => (decimal?)d.Quantity * d.UnitPrice)) ?? 0,
                    LastPurchaseDate = s.Orders.Max(d => (DateTime?)d.OrderDate)
                }).ToListAsync();
            return Ok(customerStats);
        }
        [HttpPost] 
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("don hang phai lon hon 0");
            if (dto.Items.Any(s => s.Quantity < 0))
                return BadRequest("so luong phai lon hon 0 ");
            if (!await _DB.Customers.AnyAsync(s => s.Customerid == dto.Customerid))
                return BadRequest("khong ton tai gia tri ");

            var items = dto.Items
                .GroupBy(s => s.Productid)
                .Select(s => new
                {
                    Productid = s.Key,
                    Quantity = s.Sum(d => d.Quantity)
                }).ToList();
            var ids = dto.Items.Select(a => a.Productid).ToList();
            var products = await _DB.Products
                .Where(s => ids.Contains(s.Productid))
                .ToDictionaryAsync(s => s.Productid);
            var missing = ids.Where(s => !products.ContainsKey(s)).ToList();
            if(missing.Count > 0)
            {
                return BadRequest("san pham khong ton tai trong he thong");
            }

            await using var tx = await _DB.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in items) {
                    var soluong = item.Quantity;
                    var affected = await _DB.Iventories
                        .Where(s => s.Productid == item.Productid && s.Quantity >= soluong)
                        .ExecuteUpdateAsync(s => s.SetProperty(i => i.Quantity, i => i.Quantity - soluong));


                    if(affected == 0)
                    {
                        await tx.RollbackAsync(); //nếu àm không đáp ứng thì sẽ coi như chưa có chuyện gì sảy ra 
                        return Conflict($"HET HANG '{products[item.Productid].Producname}' do luong ton kho khong du ");
                    }
                }
                var order = new Order { Customerid = dto.Customerid };
                foreach (var item in items) {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        Productid = item.Productid,
                        Quantity = item.Quantity,
                        UnitPrice = products[item.Productid].Price
                    });
                    _DB.Orders.Add(order);
                    await _DB.SaveChangesAsync();
                }
                foreach(var  item in items)
                {
                    _DB.Purchasehistories.Add(new Purchasehistory
                    {
                        Customerid = dto.Customerid,
                        Orderid = order.Orderid,
                        Productid = item.Productid,
                        Totalprice = products[item.Productid].Price * item.Quantity
                    });
                    await _DB.SaveChangesAsync();
                }
                await tx.CommitAsync();
                // ---------- BƯỚC 5: Trả kết quả ----------
                return Ok(new
                {
                    Message = "Đặt hàng thành công!",
                    order.Orderid,
                    TotalAmount = items.Sum(i => products[i.Productid].Price * i.Quantity)
                });
            } catch (Exception e)
            {
                await tx.RollbackAsync();
                return StatusCode(500, $"Lỗi hệ thống khi đặt hàng: {e.Message}");
            }

        }
       
    }
}
