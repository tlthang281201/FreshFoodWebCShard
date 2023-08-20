using Microsoft.AspNetCore.Mvc;
using ShoeShop.Exten;
using ShoeShop.ModelView;

namespace ShoeShop.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}
