using Microsoft.AspNetCore.Mvc;
using ShoeShop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Text.RegularExpressions;

namespace ShoeShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly MarketManagementContext _context;

        public ProductController(MarketManagementContext context)
        {
            _context = context;
        }
        [Route("/san-pham", Name = "Product")]
        public IActionResult Index(int? page,int CatID = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 12;

            List<Product> lsProducts = new List<Product>();
            //var lsProducts = _context.Products.AsNoTracking().Include(p => p.Cat).OrderByDescending(p=>p.ProductId);
            if (CatID != 0)
            {
                lsProducts = _context.Products.AsNoTracking()
                    .Where(x => x.CatId == CatID)
                    .OrderByDescending(x => x.DateCreated).ToList();
            }
            else
            {
                lsProducts = _context.Products.AsNoTracking()
                    .OrderByDescending(x => x.DateCreated).ToList();
            }
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            var lsCate = _context.Categories.AsNoTracking().Where(x=>x.Published == true).OrderByDescending(x => x.CatId).ToList();
            ViewBag.Cate = lsCate;
            return View(models);
        }

        public IActionResult List(int CatID, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 10;
                var lsProduct = _context.Products.AsNoTracking()
                    .Where(x=>x.CatId == CatID)
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProduct, pageNumber, pageSize);

                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index","Home");
            }
        }
        [Route("/san-pham/{Alias}-{id}", Name = "DetailProduct")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                var productlienquan = _context.Products.AsNoTracking().Where(x=>x.ProductId != id && x.CatId == product.CatId && x.Active == true).OrderByDescending(p=>p.ProductId).Take(4).ToList();
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.SPlienquan = productlienquan;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index","Home");
            }
            
        }
    }
}
