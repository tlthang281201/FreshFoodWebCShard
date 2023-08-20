using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Exten;
using ShoeShop.Models;
using ShoeShop.ModelView;

namespace ShoeShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly MarketManagementContext _context;

        public CheckoutController(MarketManagementContext context)
        {
            _context = context;
        }
        public List<CartItem> Cart
        {
            get
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (cart == default(List<CartItem>))
                {
                    cart = new List<CartItem>();
                }
                return cart;
            }
        }
        [Route("/thanh-toan",Name = "Checkout")]
        public IActionResult Index()
        {
            try
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
                var tkid = HttpContext.Session.GetString("CustomerId");
                PlaceOrderVM model = new PlaceOrderVM();
                if (tkid == null) return RedirectToAction("Login", "Account");
                if (tkid != null)
                {
                    var kh = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(tkid));
                    model.CustomerId = kh.CustomerId;
                    model.FullName = kh.FullName;
                    model.Email = kh.Email;
                    model.Phone = kh.Phone;
                    model.Address = kh.Address;
                    model.Ward = kh.Ward;
                    model.District = kh.District;
                    model.PaymentId = 1;
                }
                ViewBag.GioHang = cart;
                return View(model);
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/thanh-toan", Name = "Checkout")]
        public IActionResult Index(PlaceOrderVM muahang)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var tkid = HttpContext.Session.GetString("CustomerId");
            PlaceOrderVM model = new PlaceOrderVM();
            if (tkid != null)
            {
                var kh = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(tkid));
                model.CustomerId = kh.CustomerId;
                model.FullName = kh.FullName;
                model.Email = kh.Email;
                model.Phone = kh.Phone;
                model.Note = muahang.Note;
                kh.Address = muahang.Address;
                kh.Ward = muahang.Ward;
                kh.District = muahang.District;
                _context.Update(kh);
                _context.SaveChanges();
            }
            try
            {
                Order donhang = new Order();
                donhang.CustomerId = model.CustomerId;
                donhang.OrderDate = DateTime.Now;
                donhang.Status = "Đang xử lý";
                donhang.Deleted = false;
                donhang.Paid = false;
                donhang.Note = muahang.Note;
                donhang.PaymentId = muahang.PaymentId;
                donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.total));
                _context.Add(donhang);
                _context.SaveChanges();

                foreach (var item in cart)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = donhang.OrderId;
                    orderDetail.ProductId = item.product.ProductId;
                    orderDetail.Quantity = item.amount;
                    orderDetail.Total = Convert.ToInt32(item.total);
                    _context.Add(orderDetail);
                }
                _context.SaveChanges();
                HttpContext.Session.Remove("GioHang");
                return Redirect($"/dat-hang-thanh-cong-{donhang.OrderId}");
            }
            catch
            {
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewBag.GioHang = cart;
            return View(model);
        }

        [Route("/dat-hang-thanh-cong-{id}", Name = "Success")]
        public IActionResult Success(int id)
        {
            if (id == null) return NotFound();
            try
            {
                var tkid = HttpContext.Session.GetString("CustomerId");
                if(string.IsNullOrEmpty(tkid))
                {
                    return RedirectToAction("Login", "Account");
                }
                var khachhang = _context.Customers.AsNoTracking()
                    .SingleOrDefault(x=>x.CustomerId == Convert.ToInt32(tkid));
                var donhang = _context.Orders.Include(x=>x.Customer).FirstOrDefault(m => m.OrderId == id && Convert.ToInt32(tkid) == m.CustomerId);
                var chitietdonhang = _context.OrderDetails
                    .Include(x => x.Product).Include(x => x.Order)
                    .AsNoTracking().Where(p => p.OrderId == id)
                    .OrderBy(x => x.OrderDetailId).ToList();
                ViewBag.DonHang = donhang;
                return View(chitietdonhang);
            }
            catch
            {
                return NotFound();
            }
            
        }

    }
}
