using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
	{
		
        private readonly MarketManagementContext _context;

        public HomeController(MarketManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
		{
            var id = HttpContext.Session.GetString("Admin");
            if (id == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var sales = _context.Orders.AsNoTracking().Where(x=>x.Paid == true).Sum(x=>x.TotalMoney);
			var countOrder = _context.Orders.AsNoTracking().Count();
			var countUser = _context.Customers.Where(x => x.Active == true).AsNoTracking().Count();
			var countProduct = _context.Products.Where(x=>x.Active == true).AsNoTracking().Count();
			ViewBag.TongSanPham = countOrder;
			ViewBag.TongUser = countUser;
			ViewBag.TongDonHang = countOrder;
			ViewBag.DoanhThu = sales;
            return View();
		}
	}
}
