using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Traeq.Data;
using Traeq.Models;
using Traeq.Repositories;

namespace Traeq.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        

        public OrdersController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            
        }

        public async Task<IActionResult> Current()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _context.Orders
                .Include(o => o.Pharmacy)
                .Where(o => o.UserId == userId &&
                           (o.OrderStatus == "Pending" || o.OrderStatus == "Accepted" || o.OrderStatus == "Ready" || o.OrderStatus == "OutForDelivery"))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            
            return View(orders);
        }

        public async Task<IActionResult> History()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _context.Orders
                .Include(o => o.Pharmacy)
                .Where(o => o.UserId == userId &&
           (o.OrderStatus == "Completed" || o.OrderStatus == "Rejected" || o.OrderStatus == "Canceled"))

                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

           
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.Pharmacy)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Medicine)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            if (DateTime.Now > order.CancelAllowedUntil || order.OrderStatus != "Pending" || order.IsCanceled)
            {
                return Json(new
                {
                    success = false,
                    message = "The cancellation time has expired. If there is any issue with your order, please contact our technical support from the Contact Us page."
                });
            }

            order.IsCanceled = true;
            order.OrderStatus = "Canceled";

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Your order has been canceled successfully."
            });
        }



    }
}