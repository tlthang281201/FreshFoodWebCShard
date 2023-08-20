using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using ShoeShop.Models;
using System;
using System.Diagnostics;

namespace ShoeShop.Controllers
{
	public class HomeController : Controller
	{
        private readonly MarketManagementContext _context;

        public HomeController(MarketManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
		{
            List<Product> lsProducts = new List<Product>();
           lsProducts = _context.Products.AsNoTracking().Where(p=>(p.BestSeller == true && p.Active == true))
                    .OrderByDescending(x => x.ProductId).Take(8).ToList();
            //var lsCate = _context.Categories.AsNoTracking().OrderByDescending(x=>x.CatId).ToList();
            //ViewBag.DanhMuc = lsCate;
            return View(lsProducts);
        }
    }
}