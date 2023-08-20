using Microsoft.AspNetCore.Mvc;
using ShoeShop.Exten;
using ShoeShop.Models;
using ShoeShop.ModelView;

namespace ShoeShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly MarketManagementContext _context;

        public ShoppingCartController(MarketManagementContext context)
        {
            _context = context;
        }
        public List<CartItem> Cart
        {
            get
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if(cart == default(List<CartItem>))
                {
                    cart = new List<CartItem>();
                }
                return cart;
            }
        }
        [HttpPost]
        [Route("/api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> cart = Cart;
            try
            {
                CartItem item = Cart.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    item.amount = item.amount + amount.Value;
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                else
                {
                    Product sp = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = sp
                    };
                    cart.Add(item);
                }
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                return Json(new { success = true });
            } catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        [Route("/api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if(cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if(item!=null && amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new { success = true });
            } catch
            {
                return Json(new { success = false });
            }
        }


        [HttpPost]
        [Route("/api/cart/remove")]
        public IActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> cart = Cart;
                CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    cart.Remove(item);
                }
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                return Json(new { success = true });
            } catch
            {
                return Json(new { success = false });
            }
            
        }
        [Route("/gio-hang",Name ="Cart")]
        public IActionResult Index()
        {
            return View(Cart);
        }
    }
}
