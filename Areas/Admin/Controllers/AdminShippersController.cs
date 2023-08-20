using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using ShoeShop.Models;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminShippersController : Controller
    {
        private readonly MarketManagementContext _context;

        public AdminShippersController(MarketManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminShippers
        public IActionResult Index(int? page)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            List<Shipper> lsShipper = new List<Shipper>();
            lsShipper = _context.Shippers.AsNoTracking().OrderByDescending(x => x.ShipperId).ToList();
            PagedList<Shipper> models = new PagedList<Shipper>(lsShipper.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminShippers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Shippers == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.ShipperId == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // GET: Admin/AdminShippers/Create
        public IActionResult Create()
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            return View();
        }

        // POST: Admin/AdminShippers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShipperId,ShipperName,Phone,Company")] Shipper shipper)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shipper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shipper);
        }

        // GET: Admin/AdminShippers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Shippers == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers.FindAsync(id);
            if (shipper == null)
            {
                return NotFound();
            }
            return View(shipper);
        }

        // POST: Admin/AdminShippers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShipperId,ShipperName,Phone,Company")] Shipper shipper)
        {
            if (id != shipper.ShipperId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipperExists(shipper.ShipperId))
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
            return View(shipper);
        }

        // GET: Admin/AdminShippers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Shippers == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.ShipperId == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // POST: Admin/AdminShippers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shippers == null)
            {
                return Problem("Entity set 'MarketManagementContext.Shippers'  is null.");
            }
            var shipper = await _context.Shippers.FindAsync(id);
            if (shipper != null)
            {
                _context.Shippers.Remove(shipper);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipperExists(int id)
        {
          return _context.Shippers.Any(e => e.ShipperId == id);
        }
    }
}
