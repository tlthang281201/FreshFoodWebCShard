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
    public class AdminCustomersController : Controller
    {
        private readonly MarketManagementContext _context;

        public AdminCustomersController(MarketManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminCustomers
        public IActionResult Index(int? page, int Active = 0)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;

            List<Customer> lsCustomer = new List<Customer>();
            lsCustomer = _context.Customers.AsNoTracking().OrderByDescending(x => x.CustomerId).ToList();
            if (Active == 1)
            {
                lsCustomer = lsCustomer.Where(x => x.Active == true).ToList();
            }
            else if (Active == 2)
            {
                lsCustomer = lsCustomer.Where(x => x.Active == false).ToList();
            }

            PagedList<Customer> models = new PagedList<Customer>(lsCustomer.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/AdminCustomers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FullName,Avatar,Email,Phone,Address,District,Ward,Active,Password,CreateDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/AdminCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FullName,Avatar,Email,Phone,Address,District,Ward,Active,Password,CreateDate")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var adminid = HttpContext.Session.GetString("Admin");
            if (adminid == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/AdminCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'MarketManagementContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return _context.Customers.Any(e => e.CustomerId == id);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'MarketManagementContext.Orders'  is null.");
            }
            var kh = await _context.Customers.FindAsync(id);
            if (kh != null)
            {
                if (status == 1)
                {
                    kh.Active = false;
                    _context.Update(kh);
                }
                else
                {
                    kh.Active = true;
                    _context.Update(kh);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
