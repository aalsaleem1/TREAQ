using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Traeq.Data;
using Traeq.Models;

namespace Traeq.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    [Authorize(Roles = "Pharmacy")]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrdersController(
            AppDbContext context,
            UserManager<User> userManager
)        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<int?> GetPharmacyIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            if (user.PharmacyId != null)
            {
                return user.PharmacyId;
            }

            var pharmacy = await _context.PharmacyLegalInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            return pharmacy?.Id;
        }

        public async Task<IActionResult> Index(string search)
        {
            var pharmacyId = await GetPharmacyIdAsync();
            if (pharmacyId == null) return RedirectToAction("Login", "User", new { area = "" });

            var ordersQuery = _context.Orders
                .Include(o => o.User)
                .Where(o => o.PharmacyId == pharmacyId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                ordersQuery = ordersQuery.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.User.FullName.Contains(search) ||
                    o.User.PhoneNumber.Contains(search) ||
                    o.OrderStatus.Contains(search));
            }

            var orders = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Pending(string search)
        {
            var pharmacyId = await GetPharmacyIdAsync();
            if (pharmacyId == null) return RedirectToAction("Login", "User", new { area = "" });

            var ordersQuery = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Medicine)
                .Where(o => o.PharmacyId == pharmacyId && o.OrderStatus == "Pending");

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                ordersQuery = ordersQuery.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.User.FullName.Contains(search) ||
                    o.User.PhoneNumber.Contains(search) ||
                    o.OrderStatus.Contains(search));
            }

            var orders = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Current(string search)
        {
            var pharmacyId = await GetPharmacyIdAsync();
            if (pharmacyId == null) return RedirectToAction("Login", "User", new { area = "" });

            var ordersQuery = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Medicine)
                .Where(o => o.PharmacyId == pharmacyId &&
                           (o.OrderStatus == "Accepted" || o.OrderStatus == "Ready" || o.OrderStatus == "OutForDelivery"));

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                ordersQuery = ordersQuery.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.User.FullName.Contains(search) ||
                    o.User.PhoneNumber.Contains(search) ||
                    o.OrderStatus.Contains(search));
            }

            var orders = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Past(string search)
        {
            var pharmacyId = await GetPharmacyIdAsync();
            if (pharmacyId == null) return RedirectToAction("Login", "User", new { area = "" });

            var ordersQuery = _context.Orders
                .Include(o => o.User)
                .Where(o => o.PharmacyId == pharmacyId &&
                           (o.OrderStatus == "Completed" || o.OrderStatus == "Rejected"));

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                ordersQuery = ordersQuery.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.User.FullName.Contains(search) ||
                    o.User.PhoneNumber.Contains(search) ||
                    o.OrderStatus.Contains(search));
            }

            var orders = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pharmacyId = await GetPharmacyIdAsync();
            if (pharmacyId == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Medicine)
                .FirstOrDefaultAsync(o => o.Id == id && o.PharmacyId == pharmacyId);

            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var pharmacyId = await GetPharmacyIdAsync();
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.PharmacyId == pharmacyId);

            if (order == null) return NotFound();

            foreach (var item in order.OrderItems)
            {
                var medicine = await _context.Medicines.FindAsync(item.MedicineId);
                if (medicine != null)
                {
                    if (medicine.Quantity >= item.Quantity)
                    {
                        medicine.Quantity -= item.Quantity;
                        _context.Medicines.Update(medicine);

                        if (medicine.Quantity.HasValue && medicine.Quantity.Value <= 10)
                        {
                            var pharmacyUserId = medicine.PharmacyLegalInfo?.UserId;
                           
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Not enough stock for medicine: {medicine.MedicineName}";
                        return RedirectToAction("Pending", "Orders", new { area = "Pharmacy" });
                    }
                }
            }

            order.OrderStatus = "Accepted";
            await _context.SaveChangesAsync();


            return RedirectToAction("Current", "Orders", new { area = "Pharmacy" });
        }

        [HttpPost]
        public async Task<IActionResult> RejectOrder(int id, string reason)
        {
            var pharmacyId = await GetPharmacyIdAsync();
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.PharmacyId == pharmacyId);

            if (order == null) return NotFound();

            order.OrderStatus = "Rejected";
            order.RejectionReason = reason;
            await _context.SaveChangesAsync();


            return RedirectToAction("Past", "Orders", new { area = "Pharmacy" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var pharmacyId = await GetPharmacyIdAsync();
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.PharmacyId == pharmacyId);

            if (order == null) return NotFound();

            order.OrderStatus = status;

            if (status == "Completed")
            {
                order.IsPaid = true;
            }

            await _context.SaveChangesAsync();


            if (status == "Completed")
            {
                return RedirectToAction("Past", "Orders", new { area = "Pharmacy" });
            }

            return RedirectToAction("Current", "Orders", new { area = "Pharmacy" });
        }
    }
}
