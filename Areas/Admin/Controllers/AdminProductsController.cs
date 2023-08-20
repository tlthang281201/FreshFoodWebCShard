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
    public class AdminProductsController : Controller
    {
        private readonly MarketManagementContext _context;

        public AdminProductsController(MarketManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminProducts
        public  IActionResult Index(int? page, int CatID = 0, int Active = 0)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            List<Product> lsProducts = new List<Product>();
            //var lsProducts = _context.Products.AsNoTracking().Include(p => p.Cat).OrderByDescending(p=>p.ProductId);
            if(CatID != 0)
            {
                lsProducts = _context.Products.AsNoTracking().Where(x=>x.CatId == CatID)
                    .Include(p => p.Cat).OrderByDescending(x => x.ProductId).ToList();
            }
            else
            {
                lsProducts = _context.Products.AsNoTracking().Include(p => p.Cat)
                    .OrderByDescending(x => x.ProductId).ToList();
            }

            if (Active == 1)
            {
                lsProducts = lsProducts.Where(x => x.Active == true).ToList();
            }
            else if(Active == 2)
            {
                lsProducts = lsProducts.Where(x => x.Active == false).ToList();
            }

            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(),pageNumber,pageSize);
            
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentActive = Active;
            ViewBag.CurrentCatID = CatID;

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName",CatID);
            
            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            
            lsTrangThai.Add(new SelectListItem() { Text="Hiển thị",Value="1" });
            lsTrangThai.Add(new SelectListItem() { Text = "Đã ẩn", Value = "2" });
            
            ViewData["TrangThai"] = lsTrangThai;
            return View(models);
        }

        public IActionResult Filter(int CatID = 0, int Active = 0)
        {
            var url = $"/Admin/AdminProducts?CatID={CatID}&Active={Active}";
            if(CatID == 0 && Active == 0)
            {
                url = $"/Admin/AdminProducts";
            } else
            {
                if (Active == 0) url = $"/Admin/AdminProducts?CatID={CatID}";
                if (CatID == 0) url = $"/Admin/AdminProducts?Active={Active}";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile Thumb)
        {
            product.ProductName = Extension.ToTitleCase(product.ProductName);
            if(Thumb != null)
            {
                string ext = Path.GetExtension(Thumb.FileName);
                string img = Utinities.GetRandomKey(10) + ext;
                product.Thumb = await Utinities.UploadFile(Thumb, @"products", img);
            }
            product.Alias = Utinities.SEOUrl(product.ProductName);
            product.DateCreated = DateTime.Now;
            product.DateModified = DateTime.Now;
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Product product,IFormFile Thumb)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }
            try
            {
                var productOld = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProductId == id);

                product.ProductName = Extension.ToTitleCase(product.ProductName);
                if (Thumb != null)
                {
                    string ext = Path.GetExtension(Thumb.FileName);
                    string img = Utinities.GetRandomKey(10) + ext;
                    product.Thumb = await Utinities.UploadFile(Thumb, @"products", img);
                }
                if(string.IsNullOrEmpty(product.Thumb)) { product.Thumb = productOld.Thumb; }
                product.Alias = Utinities.SEOUrl(product.ProductName);
                product.DateModified = DateTime.Now;
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
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

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'MarketManagementContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
