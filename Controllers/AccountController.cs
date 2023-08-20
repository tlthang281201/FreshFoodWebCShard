using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Exten;
using ShoeShop.Helper;
using ShoeShop.Models;
using ShoeShop.ModelView;
using System.Security.Claims;

namespace ShoeShop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly MarketManagementContext _context;

        public AccountController(MarketManagementContext context)
        {
            _context = context;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string phone)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Phone.ToLower() == phone.ToLower());
                if(khachhang != null)
                {
                    return Json(data: "Số điện thoại "+phone+" đã được sử dụng");
                }
                return Json(data: true);
            }catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string email)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
                if (khachhang != null)
                {
                    return Json(data: "Email " + email + " đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [Route("/thong-tin-ca-nhan", Name = "Profile")]
        public IActionResult Index()
        {
            var taikhoanid = HttpContext.Session.GetString("CustomerId");
            if (taikhoanid != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanid));
                if(khachhang != null)
                {
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        [Route("/thong-tin-ca-nhan", Name = "Profile")]
        public IActionResult Index(Customer customer)
        {
            if (customer == null) return View(customer);
            var taikhoanid = HttpContext.Session.GetString("CustomerId");
            if (taikhoanid != null)
            {
                var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanid));
                if (taikhoan != null)
                {
                    taikhoan.FullName = customer.FullName;
                    taikhoan.Address = customer.Address;
                    taikhoan.Ward = customer.Ward;
                    taikhoan.District = customer.District;
                    _context.Update(taikhoan);
                    _context.SaveChanges();
                    return View(taikhoan);
                }
            }
            return RedirectToAction("Login", "Account");
        }
        [Route("/danh-sach-don-hang", Name = "OrderList")]
        public IActionResult OrderList()
        {
            var taikhoanid = HttpContext.Session.GetString("CustomerId");
            if (taikhoanid != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanid));
                if (khachhang != null)
                {
                    var lsDonhang = _context.Orders
                        .AsNoTracking()
                        .Where(x => x.CustomerId == khachhang.CustomerId)
                        .OrderByDescending(x=>x.OrderDate).ToList();
                    ViewBag.Donhang = lsDonhang;
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("/dang-ky",Name = "ViewDangKy")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/dang-ky", Name = "DangKy")]
        public async Task<IActionResult> Register(Register taikhoan)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    Customer khachhang = new Customer
                    {
                        FullName = taikhoan.FullName,
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password).CreateMD5(),
                        Avatar = "https://www.w3schools.com/howto/img_avatar.png",
                        Active = true,
                        CreateDate = DateTime.Now,
                    };
                    try
                    {
                        var kh = _context.Customers.AsNoTracking()
                        .FirstOrDefault(x => x.Email.ToLower() == taikhoan.Email.ToLower());
                        if(kh != null)
                        {
                            ViewBag.Error1 = "Email đã được sử dụng";
                            return View(taikhoan);
                        }
                        var kh2 = _context.Customers.AsNoTracking()
                        .FirstOrDefault(x => x.Phone.ToLower() == taikhoan.Phone.ToLower());
                        if (kh2 != null)
                        {
                            ViewBag.Error2 = "Số điện thoại đã được sử dụng";
                            return View(taikhoan);
                        }

                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        //Lưu session ma khach hang
                        HttpContext.Session.SetString("CustomerId",khachhang.CustomerId.ToString());
                        var taikhoanid = HttpContext.Session.GetString("CustomerId");
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, khachhang.FullName),
                            new Claim("CustomerId",khachhang.CustomerId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,"login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("Index", "Account");
                    } catch (Exception ex)
                    {
                        return RedirectToAction("Register", "Account");
                    }
                }
                else
                {
                    return View(taikhoan);
                }

            } catch
            {
                return View(taikhoan);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("/dang-nhap", Name = "DangNhap")]
        public IActionResult Login()
        {
            var taikhoanid = HttpContext.Session.GetString("CustomerId");
            if(taikhoanid != null)
            {
                return RedirectToAction("Index", "Account");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/dang-nhap", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel customer)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    bool isEmail = Utinities.IsValidEmail(customer.Email);
                    if(!isEmail)
                    {
                        return View(customer);
                    }
                    var khachhang = _context.Customers.AsNoTracking()
                        .SingleOrDefault(x=>x.Email.Trim() == customer.Email.Trim());
                    if(khachhang == null)
                    {
                        ViewBag.Error = "Sai thông tin đăng nhập";
                        return View(customer);
                    }
                    string pass = (customer.Password).CreateMD5();
                    if(khachhang.Password != pass)
                    {
                        ViewBag.Error = "Sai thông tin đăng nhập";
                        return View(customer);
                    }
                    if(khachhang.Active == false)
                    {
                        ViewBag.Error = "Tài khoản của bạn đã bị khoá";
                        return View(customer);
                    }
                    HttpContext.Session.SetString("CustomerId",khachhang.CustomerId.ToString());
                    var taikhoanid = HttpContext.Session.GetString("CustomerId");
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,khachhang.FullName),
                        new Claim("CustomerId",khachhang.CustomerId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToAction("Index", "Account");
                }
            } catch(Exception ex)
            {
                return RedirectToAction("Register", "Account");
            }
            return View(customer);
        }
        [HttpGet]
        [Route("/dang-xuat",Name ="Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        [Route("/thay-doi-mat-khau", Name = "ChangePassword")]
        public IActionResult ChangePassword()
        {
            var taikhoanid = HttpContext.Session.GetString("CustomerId");
            if (taikhoanid != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanid));
                if (khachhang != null)
                {
                    ViewBag.Name = khachhang.FullName;
                    ViewBag.Avatar = khachhang.Avatar;
                    return View();
                }
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        [Route("/thay-doi-mat-khau", Name = "ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordViewModel customer)
        {
            try
            {
                var taikhoanid = HttpContext.Session.GetString("CustomerId");
                if (taikhoanid == null) return RedirectToAction("Login", "Account");
                var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanid));
                if (ModelState.IsValid)
                {
                    if (taikhoan == null) return RedirectToAction("Login", "Account");
                    var pass = customer.PasswordNow.Trim().CreateMD5();
                    if(customer.Password.Trim() == customer.PasswordNow.Trim())
                    {
                        ViewBag.Name = taikhoan.FullName;
                        ViewBag.Avatar = taikhoan.Avatar;
                        ViewBag.Error2 = "Mật khẩu mới không được giống mật khẩu cũ";
                        return View(customer);
                    }
                    if (taikhoan.Password == pass)
                    {
                        string newpass = customer.Password.Trim().CreateMD5();
                        taikhoan.Password = newpass;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        ViewBag.Name = taikhoan.FullName;
                        ViewBag.Avatar = taikhoan.Avatar;
                        ViewBag.Success = "Thay đổi mật khẩu thành công";
                        return View(customer);
                    }
                    else
                    {
                        ViewBag.Name = taikhoan.FullName;
                        ViewBag.Avatar = taikhoan.Avatar;
                        ViewBag.Error = "Mật khẩu không chính xác";
                        return View(customer);
                    }
                }
                ViewBag.Name = taikhoan.FullName;
                ViewBag.Avatar = taikhoan.Avatar;
                return View(customer);
            } catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
    }
}
