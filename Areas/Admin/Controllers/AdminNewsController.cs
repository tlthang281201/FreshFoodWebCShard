using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using ShoeShop.Helper;
using ShoeShop.Models;
using ShoeShop.Exten;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminNewsController : Controller
    {
        private readonly MarketManagementContext _context;

        public AdminNewsController(MarketManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminNews
        public IActionResult Index(int? page, int Active = 0)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            List<News> lsNews = new List<News>();
            lsNews = _context.News.AsNoTracking().OrderByDescending(x => x.PostId).ToList();
            if (Active == 1)
            {
                lsNews = lsNews.Where(x => x.Published == true).ToList();
            }
            else if (Active == 2)
            {
                lsNews = lsNews.Where(x => x.Published == false).ToList();
            }

            PagedList<News> models = new PagedList<News>(lsNews.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentActive = Active;

            return View(models);
        }

        // GET: Admin/AdminNews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: Admin/AdminNews/Create
        public IActionResult Create()
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            return View();
        }

        // POST: Admin/AdminNews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,DateCreated,Views")] News news, IFormFile Thumb)
        {
            if (Thumb != null)
            {
                string ext = Path.GetExtension(Thumb.FileName);
                string img = Utinities.GetRandomKey(10) + ext;
                news.Thumb = await Utinities.UploadFile(Thumb, @"news", img);
            }
            news.Views = 0;
            news.Title = Extension.ToTitleCase(news.Title);
            news.Alias = Utinities.SEOUrl(news.Title);
            news.DateCreated = DateTime.Now;
            _context.Add(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminNews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: Admin/AdminNews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,News news,IFormFile Thumb)
        {
            if (id != news.PostId)
            {
                return NotFound();
            }

            try
            {
                if (Thumb != null)
                {
                    string ext = Path.GetExtension(Thumb.FileName);
                    string img = Utinities.GetRandomKey(10) + ext;
                    news.Thumb = await Utinities.UploadFile(Thumb, @"news", img);
                }
                news.Title = Extension.ToTitleCase(news.Title);
                news.Alias = Utinities.SEOUrl(news.Title);
                _context.Update(news);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(news.PostId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminNews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: Admin/AdminNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'MarketManagementContext.News'  is null.");
            }
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
          return _context.News.Any(e => e.PostId == id);
        }
    }
}
