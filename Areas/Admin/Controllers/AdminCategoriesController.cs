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
    public class AdminCategoriesController : Controller
    {
        private readonly MarketManagementContext _context;

        public AdminCategoriesController(MarketManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminCategories
        public IActionResult Index(int? page, int Active = 0)
        {
            var id = HttpContext.Session.GetString("Admin");
            if (id == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            List<Category> lsCate = new List<Category>();
            lsCate = _context.Categories.AsNoTracking().OrderByDescending(x => x.CatId).ToList();
            if (Active == 1)
            {
                lsCate = lsCate.Where(x => x.Published == true).ToList();
            }
            else if (Active == 2)
            {
                lsCate = lsCate.Where(x => x.Published == false).ToList();
            }

            PagedList<Category> models = new PagedList<Category>(lsCate.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentActive = Active;

            return View(models);
        }

        // GET: Admin/AdminCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/AdminCategories/Create
        public IActionResult Create()
        {
            var id = HttpContext.Session.GetString("Admin");
            if (id == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            return View();
        }

        // POST: Admin/AdminCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Description,Published,Alias")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CatName = Extension.ToTitleCase(category.CatName);
                category.Alias = Utinities.SEOUrl(category.CatName);
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/AdminCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName,Description,Published,Alias")] Category category)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.CatName = Extension.ToTitleCase(category.CatName);
                    category.Alias = Utinities.SEOUrl(category.CatName);
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CatId))
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
            return View(category);
        }

        // GET: Admin/AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'MarketManagementContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return _context.Categories.Any(e => e.CatId == id);
        }
    }
}
