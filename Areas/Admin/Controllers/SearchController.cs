using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        
        private readonly MarketManagementContext _context;

        public SearchController(MarketManagementContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> products = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                products = _context.Products.AsNoTracking().Include(p => p.Cat)
                    .OrderByDescending(x => x.ProductId).ToList();
                return PartialView("ListProductSearchPartial", products);
            }
            string keyword2 = Regex.Replace(keyword.Trim(), @"\s+", " ");
            
            products = _context.Products.AsNoTracking().Include(a=>a.Cat)
                .Where(x=>x.ProductName.Contains(keyword2)).OrderByDescending(x=>x.ProductName)
                .Take(10).ToList();
            if(products == null || products.Count == 0)
            {
                return PartialView("ListProductSearchPartial", null);
            }
            else
            {
                return PartialView("ListProductSearchPartial", products);
            }
        }
    }
}
