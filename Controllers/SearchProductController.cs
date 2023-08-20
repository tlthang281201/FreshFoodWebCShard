using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Models;
using System.Text.RegularExpressions;

namespace ShoeShop.Controllers
{
    public class SearchProductController : Controller
    {
        private readonly MarketManagementContext _context;

        public SearchProductController(MarketManagementContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult FindProduct(string keyword, int CatID = 0)
        {
            List<Product> products = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                products = _context.Products.AsNoTracking().Include(p => p.Cat)
                    .OrderByDescending(x => x.ProductId).ToList();
                return PartialView("ProductSearchPartial", products);
            }
            string keyword2 = Regex.Replace(keyword.Trim(), @"\s+", " ");
            if(CatID != 0)
            {
                products = _context.Products.AsNoTracking().Include(a => a.Cat)
                .Where(x => x.CatId == CatID && x.ProductName.Contains(keyword2)).OrderByDescending(x => x.ProductName)
                .Take(10).ToList();
            } else
            {
                products = _context.Products.AsNoTracking().Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword2)).OrderByDescending(x => x.ProductName)
                .Take(10).ToList();
            }
            
            if (products == null || products.Count == 0)
            {
                return PartialView("ProductSearchPartial", null);
            }
            else
            {
                return PartialView("ProductSearchPartial", products);
            }
        }
    }
}
