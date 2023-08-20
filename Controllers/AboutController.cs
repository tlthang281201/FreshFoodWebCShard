using Microsoft.AspNetCore.Mvc;

namespace ShoeShop.Controllers
{
    public class AboutController : Controller
    {
        [Route("/ve-chung-toi", Name = "About")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
