using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using ShoeShop.Models;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly MarketManagementContext _context;

        public AdminOrdersController(MarketManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminOrders
        public IActionResult Index(int? page, int Active = 0)
        {
            var id = HttpContext.Session.GetString("Admin");
            if (id == null)
            {
                return RedirectToAction("Index", "AdminLogin");
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            List<Order> lsOrder = new List<Order>();
            lsOrder = _context.Orders.AsNoTracking().Include(x=>x.Customer).Include(x=>x.Shipper).OrderByDescending(x => x.OrderId).ToList();
            PagedList<Order> models = new PagedList<Order>(lsOrder.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            var orderDetail = _context.OrderDetails.Include(x => x.Product)
                .Where(x => x.OrderId == id).AsNoTracking().ToList();
            if (order == null)
            {
                return NotFound();
            }
            ViewData["Customer"] = order.Customer;
            ViewData["Shipper"] = new SelectList(_context.Shippers, "ShipperId", "ShipperName", order.ShipperId);
            ViewData["OrderDetail"] = orderDetail;
            return View(order);
        }

        // GET: Admin/AdminOrders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["ShipperId"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId");
            return View();
        }

        // POST: Admin/AdminOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,ShipDate,Status,Deleted,Paid,PaymentDate,PaymentId,Note,ShipperId,TotalMoney")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["ShipperId"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipperId);
            return View(order);
        }

        // GET: Admin/AdminOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order =  _context.Orders.Include(x=>x.Customer).Where(x=>x.OrderId == id).FirstOrDefault();
            var orderDetail = _context.OrderDetails.Include(x => x.Product)
                .Where(x => x.OrderId == id).AsNoTracking().ToList();
            if (order == null)
            {
                return NotFound();
            }
            ViewData["Customer"] = order.Customer;
            ViewData["Shipper"] = new SelectList(_context.Shippers, "ShipperId", "ShipperName", order.ShipperId);
            ViewData["OrderDetail"] = orderDetail;
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Giao hang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            try
            {
                order.ShipDate = DateTime.Now;
                order.Status = "Đang giao hàng";
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderId))
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

        public IActionResult ConfirmOrder(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = _context.Orders.Include(x => x.Customer).Where(x => x.OrderId == id).FirstOrDefault();
            var orderDetail = _context.OrderDetails.Include(x => x.Product)
                .Where(x => x.OrderId == id).AsNoTracking().ToList();
            if (order == null)
            {
                return NotFound();
            }
            ViewData["Customer"] = order.Customer;
            ViewData["OrderDetail"] = orderDetail;
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Giao hang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(int id,int status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                try
                {
                    if (status == 1)
                    {
                        order.Paid = true;
                        order.PaymentDate = DateTime.Now;
                        order.Status = "Chờ giao hàng";
                    }
                    else
                    {
                        order.Status = "Đã huỷ";
                        order.Deleted = true;
                    }
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
                
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CompleteOrder(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = _context.Orders.Include(x => x.Customer).Where(x => x.OrderId == id).FirstOrDefault();
            var orderDetail = _context.OrderDetails.Include(x => x.Product)
                .Where(x => x.OrderId == id).AsNoTracking().ToList();
            if (order == null)
            {
                return NotFound();
            }
            ViewData["Customer"] = order.Customer;
            ViewData["OrderDetail"] = orderDetail;
            ViewData["Shipper"] = new SelectList(_context.Shippers, "ShipperId", "ShipperName", order.ShipperId);
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //Giao hang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                try
                {
                    order.Status = "Đã giao";
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id,int status)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'MarketManagementContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                if(status == 1)
                {
                    order.Paid = true;
                    order.PaymentDate = DateTime.Now;
                    order.Status = "Chờ giao hàng";
                    _context.Update(order);
                } else if(status == 2)
                {
                    order.Status = "Đang giao hàng";
                    _context.Update(order);
                } else if(status == 3)
                {
                    order.Status = "Đã giao";
                    _context.Update(order);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Admin/AdminOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/AdminOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'MarketManagementContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = "Đã huỷ";
                order.Deleted = true;
                _context.Update(order);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          
          return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
