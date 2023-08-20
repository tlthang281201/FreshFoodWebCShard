using Microsoft.AspNetCore.Mvc;

namespace ShoeShop.Controllers
{
    public class ContactController : Controller
    {
        [Route("/lien-he", Name = "Contact")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
