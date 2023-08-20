using Microsoft.AspNetCore.Mvc;
using ShoeShop.Models;
using ShoeShop.Exten;
using Microsoft.EntityFrameworkCore;

namespace ShoeShop.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly MarketManagementContext _context;

        public OrderDetailController(MarketManagementContext context)
        {
            _context = context;
        }
        [Route("/chi-tiet-don-hang-{id}",Name = "OrderDetail")]
        public IActionResult Index(int? id)
        {
            if(id == null) return NotFound();
            try
            {
                var taikhoanid = HttpContext.Session.GetString("CustomerId");
                if(string.IsNullOrEmpty(taikhoanid)) return RedirectToAction("Login","Account");
                var kh = _context.Customers.AsNoTracking().SingleOrDefault(x=>x.CustomerId == Convert.ToInt32(taikhoanid));
                if(kh == null) return NotFound();
                var donhang = _context.Orders.FirstOrDefault(m=>m.OrderId== id && Convert.ToInt32(taikhoanid) == m.CustomerId);
                if(donhang == null) return NotFound();

                var khachhang = _context.Customers
                    .AsNoTracking().SingleOrDefault(x => x.CustomerId == donhang.CustomerId);
                var chitietdonhang = _context.OrderDetails
                    .Include(x => x.Product).Include(x => x.Order)
                    .AsNoTracking().Where(p => p.OrderId == id)
                    .OrderBy(x => x.OrderDetailId).ToList();

                ViewBag.KhachHang = khachhang;
                ViewBag.DonHang = donhang;
                return View(chitietdonhang);
            } catch
            {
                return NotFound();
            }
            
        }
    }
}
