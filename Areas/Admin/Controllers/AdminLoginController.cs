using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Exten;
using ShoeShop.Helper;
using ShoeShop.Models;
using ShoeShop.ModelView;
using System.Security.Claims;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLoginController : Controller
    {
        private readonly MarketManagementContext _context;

        public AdminLoginController(MarketManagementContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetString("Admin");
            if (id != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utinities.IsValidEmail(model.Email);
                    if (!isEmail)
                    {
                        return View();
                    }
                    var admin = _context.Accounts.AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == model.Email.Trim());
                    if (admin == null)
                    {
                        ViewBag.Error = "Sai thông tin đăng nhập";
                        return View();
                    }
                    string pass = (model.Password).CreateMD5();
                    if (admin.Password != pass)
                    {
                        ViewBag.Error = "Sai thông tin đăng nhập";
                        return View();
                    }
                    HttpContext.Session.SetString("Admin", admin.AccountId.ToString());
                    var taikhoanid = HttpContext.Session.GetString("Admin");
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,"admin123"),
                        new Claim("Admin",admin.AccountId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}
