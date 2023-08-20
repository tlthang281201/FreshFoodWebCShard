using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using ShoeShop.Models;

namespace ShoeShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly MarketManagementContext _context;

        public BlogController(MarketManagementContext context)
        {
            _context = context;
        }
        [Route("/tin-tuc", Name = "Blog")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            
            var lsNews = _context.News.AsNoTracking().OrderByDescending(x => x.PostId).Where(p => p.Published == true);
            
            PagedList<News> models = new PagedList<News>(lsNews, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        [Route("/tin-tuc/{Alias}-{id}",Name = "BlogDetails")]
        public  IActionResult Details(int? id)
        {
           
            var tindang = _context.News.AsNoTracking().SingleOrDefault(x=>x.PostId == id);
            if(tindang == null)
            {
                return RedirectToAction("Index");
            }
            tindang.Views += 1;
            _context.Update(tindang);
            _context.SaveChanges();
            var lsNews = _context.News.AsNoTracking().OrderByDescending(x => x.PostId).Where(p => p.Published == true && p.PostId != id).Take(5).ToList();
            ViewBag.TinLienQuan = lsNews;
            return View(tindang);
        }
    }
}
