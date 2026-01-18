using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Traeq.Areas.Pharmacy.ViewModels;
using Traeq.Data;
using Traeq.DTO;
using Traeq.Models;

namespace Traeq.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    [Authorize(Roles = "Pharmacy")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<PharmacyLegalInfo?> GetCurrentPharmacyAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return null;

            if (user.PharmacyId != null)
            {
                if (!user.IsActive || user.IsDelete) return null;

                return await _context.PharmacyLegalInfos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == user.PharmacyId);
            }

            return await _context.PharmacyLegalInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IActionResult> Index()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Login", "User", new { area = "" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);

            if (currentUser.PharmacyId != null && !currentUser.CanViewDashboard)
            {
                return RedirectToAction("AccessDenied", "User", new { area = "" });
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var totalMedicines = await _context.Medicines
                .CountAsync(m => m.PharmacyLegalInfoId == pharmacy.Id && !m.IsDelete);

            var lowStockCount = await _context.Medicines
                .CountAsync(m => m.PharmacyLegalInfoId == pharmacy.Id
                                 && m.Quantity < 10
                                 && !m.IsDelete);

            var dailyRevenue = await _context.Orders
                .Where(o => o.PharmacyId == pharmacy.Id
                            && o.OrderDate >= today && o.OrderDate < tomorrow
                            && o.OrderStatus != "Rejected"
                            && o.OrderStatus != "Pending")
                .SumAsync(o => o.TotalAmount);

            var dailyOrders = await _context.Orders
                .CountAsync(o => o.PharmacyId == pharmacy.Id
                            && o.OrderDate >= today && o.OrderDate < tomorrow);

            var recentOrdersList = await _context.Orders
                .Where(o => o.PharmacyId == pharmacy.Id)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new RecentOrderViewModel
                {
                    OrderNumber = o.Id,
                    CustomerName = o.User.FullName,
                    Amount = o.TotalAmount,
                    Status = o.OrderStatus,
                    Date = o.OrderDate
                })
                .ToListAsync();

            var model = new PharmacyDashboardViewModel
            {
                PharmacyName = pharmacy.PharmacyName,
                LicenseNumber = pharmacy.LicenseNumber,
                OwnerName = pharmacy.OwnerName,
                LogoUrl = pharmacy.PharmacyLogoUrl,
                City = pharmacy.User?.City ?? "Amman",
                TotalMedicines = totalMedicines,
                LowStockCount = lowStockCount,
                DailyRevenue = dailyRevenue,
                DailyOrdersCount = dailyOrders,
                RecentOrders = recentOrdersList
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyMessages()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Index");

            var threads = await _context.PharmacySupportThreads
                .Include(t => t.User)
                .Include(t => t.Order)
                .Include(t => t.Messages)
                .Where(t => t.PharmacyId == pharmacy.Id)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(threads);
        }

        [HttpGet]
        public async Task<IActionResult> Chat(int threadId)
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Index");

            var thread = await _context.PharmacySupportThreads
                .Include(t => t.User)
                .Include(t => t.Order)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t => t.Id == threadId && t.PharmacyId == pharmacy.Id);

            if (thread == null) return NotFound();

            var title = $"Pharmacy Support - {thread.User?.FullName}";
            if (thread.OrderId.HasValue)
                title = $"Order #{thread.OrderId} - {thread.User?.FullName}";

            var subTitle = thread.OrderId.HasValue
                ? $"Order #: {thread.OrderId}"
                : "General question";

            var messages = thread.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => new SupportChatMessageViewModel
                {
                    SenderType = m.SenderType,
                    SenderLabel = m.SenderType == "User" ? "User" : "You",
                    MessageText = m.MessageText,
                    SentAt = m.SentAt
                })
                .ToList();

            var vm = new SupportChatViewModel
            {
                ThreadId = thread.Id,
                Title = title,
                SubTitle = subTitle,
                IsClosed = thread.IsClosed,
                BackLabel = "My Messages",
                BackController = "Dashboard",
                BackAction = "MyMessages",
                SendAction = "SendPharmacySupportMessage",
                CloseAction = "ClosePharmacySupportThread",
                Messages = messages
            };

            return View("Chat", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Employees()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return Redirect("/Pharmacy/Dashboard/Index");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);

            if (currentUser.PharmacyId != null)
                return Redirect("/Pharmacy/Dashboard/Index");

            var employees = await _userManager.Users
                .Where(u => u.PharmacyId == pharmacy.Id && !u.IsDelete)
                .ToListAsync();

            return View(employees);
        }

        [HttpGet]
        public IActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return Redirect("/Pharmacy/Dashboard/Index");

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                AccountType = "Employee",
                IsActive = true,
                CreateDate = DateTime.Now,
                PharmacyId = pharmacy.Id,
                CanAddMedicine = model.CanAdd,
                CanEditMedicine = model.CanEdit,
                CanDeleteMedicine = model.CanDelete,
                CanViewDashboard = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Pharmacy");
                return RedirectToAction("Employees", "Dashboard", new { area = "Pharmacy" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsDelete = true;
                user.IsActive = false;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Employees");
        }
    }
}
