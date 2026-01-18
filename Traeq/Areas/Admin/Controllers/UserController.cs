using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Traeq.Data;
using Traeq.DTO;
using Traeq.Models;

namespace Traeq.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;
       

        public UserController(
            UserManager<User> userManager,
            IWebHostEnvironment webHostEnvironment,
            AppDbContext context
           )
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreateDate)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> PharmaciesList()
        {
            var pharmacies = await _userManager.Users
                .Where(u => u.AccountType == "Pharmacy")
                .OrderByDescending(u => u.CreateDate)
                .ToListAsync();
            return View("Index", pharmacies);
        }

        public async Task<IActionResult> RegularUsers()
        {
            var users = await _userManager.Users
                .Where(u => u.AccountType == "User")
                .OrderByDescending(u => u.CreateDate)
                .ToListAsync();
            return View("Index", users);
        }

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromForm] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                AccountType = "Admin",
                IsActive = true,
                CreateDate = DateTime.Now
            };

            if (model.UserFile != null)
            {
                user.ImageUrl = SaveImage(model.UserFile);
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Json(new { success = true, message = "Admin created successfully" });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Json(new { success = false, message = errors });
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.PharmacyLegalInfo)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            PharmacyLegalInfo? pharmacy = null;

            if (user.AccountType == "Pharmacy")
            {
                pharmacy = await _context.PharmacyLegalInfos
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);
            }

            var model = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                District = user.District,
                AccountType = user.AccountType,
                IsActive = user.IsActive,
                ImageUrl = user.ImageUrl,
                PharmacyName = pharmacy?.PharmacyName,
                PharmacyLicense = pharmacy?.LicenseNumber,
                OwnerName = pharmacy?.OwnerName,
                PharmacyLogoUrl = pharmacy?.PharmacyLogoUrl
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var pharmacy = await _context.PharmacyLegalInfos.FirstOrDefaultAsync(p => p.UserId == user.Id);

            var model = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                District = user.District,
                AccountType = user.AccountType,
                IsActive = user.IsActive,
                ImageUrl = user.ImageUrl,
                PharmacyName = pharmacy?.PharmacyName,
                PharmacyLicense = pharmacy?.LicenseNumber,
                OwnerName = pharmacy?.OwnerName,
                PharmacyLogoUrl = pharmacy?.PharmacyLogoUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(new { success = false, message = string.IsNullOrEmpty(errors) ? "Invalid data." : errors });
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.City = model.City;
            user.District = model.District;
            user.EditId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            user.EditDate = DateTime.Now;

            if (model.UserFile != null)
            {
                user.ImageUrl = SaveImage(model.UserFile);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var identityErrors = string.Join("; ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = identityErrors });
            }

            if (user.AccountType == "Pharmacy")
            {
                var pharmacy = await _context.PharmacyLegalInfos.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (pharmacy != null)
                {
                    pharmacy.PharmacyName = model.PharmacyName;
                    pharmacy.LicenseNumber = model.PharmacyLicense;
                    pharmacy.OwnerName = model.OwnerName;

                    if (model.LogoFile != null)
                    {
                        pharmacy.PharmacyLogoUrl = SaveImage(model.LogoFile);
                    }

                    _context.PharmacyLegalInfos.Update(pharmacy);
                    await _context.SaveChangesAsync();
                }
            }

            return Json(new { success = true, message = "User updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded && user.AccountType == "Pharmacy")
            {
                var pharmacy = await _context.PharmacyLegalInfos.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (pharmacy != null)
                {
                    pharmacy.IsActive = user.IsActive;
                    _context.PharmacyLegalInfos.Update(pharmacy);
                    await _context.SaveChangesAsync();
                }
            }

           

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.PharmacyLegalInfo)
                .Include(u => u.EmployedAtPharmacy)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return RedirectToAction("Index");

            PharmacyLegalInfo? pharmacy = null;

            if (user.AccountType == "Pharmacy")
            {
                if (user.PharmacyLegalInfo != null && user.PharmacyLegalInfo.Any())
                    pharmacy = user.PharmacyLegalInfo.FirstOrDefault();

                if (pharmacy == null && user.EmployedAtPharmacy != null)
                    pharmacy = user.EmployedAtPharmacy;

                if (pharmacy == null && user.PharmacyId.HasValue)
                {
                    pharmacy = await _context.PharmacyLegalInfos
                        .FirstOrDefaultAsync(p => p.Id == user.PharmacyId.Value);
                }

                if (pharmacy == null)
                {
                    pharmacy = await _context.PharmacyLegalInfos
                        .FirstOrDefaultAsync(p => p.UserId == user.Id);
                }
            }
            else if (user.AccountType == "Employee")
            {
                if (user.EmployedAtPharmacy != null)
                    pharmacy = user.EmployedAtPharmacy;

                if (pharmacy == null && user.PharmacyId.HasValue)
                {
                    pharmacy = await _context.PharmacyLegalInfos
                        .FirstOrDefaultAsync(p => p.Id == user.PharmacyId.Value);
                }
            }

            if (pharmacy != null)
            {
                var pharmacyId = pharmacy.Id;

                var employees = await _userManager.Users
                    .Where(u => u.PharmacyId == pharmacyId || u.EmployedAtPharmacy != null && u.EmployedAtPharmacy.Id == pharmacyId)
                    .ToListAsync();

                if (employees.Any())
                {
                    foreach (var emp in employees)
                    {
                        var empRoles = await _userManager.GetRolesAsync(emp);
                        if (empRoles.Any())
                        {
                            await _userManager.RemoveFromRolesAsync(emp, empRoles);
                        }
                        _context.Users.Remove(emp);
                    }
                }

                var medicines = await _context.Medicines
                    .Where(m => m.PharmacyLegalInfoId == pharmacyId)
                    .ToListAsync();
                _context.Medicines.RemoveRange(medicines);

                var orders = await _context.Orders
                    .Where(o => o.PharmacyId == pharmacyId)
                    .ToListAsync();
                _context.Orders.RemoveRange(orders);

                var medicineIds = medicines.Select(m => m.Id).ToList();
                if (medicineIds.Any())
                {
                    var cartItems = await _context.Carts
                        .Where(c => c.MedicineId != null && medicineIds.Contains(c.MedicineId))
                        .ToListAsync();
                    _context.Carts.RemoveRange(cartItems);
                }

                _context.PharmacyLegalInfos.Remove(pharmacy);
            }

            var addresses = await _context.UserAddresses
                .Where(a => a.UserId == user.Id)
                .ToListAsync();
            _context.UserAddresses.RemoveRange(addresses);

            var userOrders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .ToListAsync();
            _context.Orders.RemoveRange(userOrders);

            var userCarts = await _context.Carts
                .Where(c => c.UserId == user.Id)
                .ToListAsync();
            _context.Carts.RemoveRange(userCarts);

            var rolesForUser = await _userManager.GetRolesAsync(user);
            if (rolesForUser.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, rolesForUser);
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            term = term?.Trim();

            IQueryable<User> query = _userManager.Users;

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(u =>
                    u.UserName.Contains(term) ||
                    u.FullName.Contains(term) ||
                    u.Email.Contains(term) ||
                    u.PhoneNumber.Contains(term) ||
                    u.Id.Contains(term));
            }

            var users = await query
                .OrderByDescending(u => u.CreateDate)
                .ToListAsync();

            return View("Index", users);
        }

        public async Task<IActionResult> SiteMessages()
        {
            var messages = await _context.ContactUss
                .OrderByDescending(c => c.CreateDate)
                .ToListAsync();

            return View(messages);
        }

        public async Task<IActionResult> OrderMessages()
        {
            var threads = await _context.OrderSupportThreads
                .Include(t => t.Order)
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(threads);
        }

        public async Task<IActionResult> OrderMessageDetails(int id)
        {
            var thread = await _context.OrderSupportThreads
                .Include(t => t.Order)
                .Include(t => t.User)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (thread == null)
                return NotFound();

            thread.Messages = thread.Messages.OrderBy(m => m.SentAt).ToList();

            return View(thread);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAdminOrderSupportMessage(int threadId, string messageText)
        {
            var thread = await _context.OrderSupportThreads
                .FirstOrDefaultAsync(t => t.Id == threadId && !t.IsClosed);

            if (thread == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(messageText))
                return RedirectToAction("OrderMessageDetails", new { id = threadId });

            var adminId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (thread.AdminId == null)
            {
                thread.AdminId = adminId;
            }

            var msg = new OrderSupportMessage
            {
                ThreadId = thread.Id,
                SenderType = "Admin",
                MessageText = messageText,
                SentAt = DateTime.Now
            };

            _context.OrderSupportMessages.Add(msg);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderMessageDetails", new { id = threadId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseAdminOrderSupportThread(int threadId)
        {
            var thread = await _context.OrderSupportThreads
                .FirstOrDefaultAsync(t => t.Id == threadId && !t.IsClosed);

            if (thread == null)
                return NotFound();

            thread.IsClosed = true;
            thread.ClosedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderMessages");
        }

        public string SaveImage(IFormFile File)
        {
            string FilePath = "images";
            string ImageName = "";
            if (File != null)
            {
                string ImagePath = Path.Combine(_webHostEnvironment.WebRootPath, FilePath);
                if (!Directory.Exists(ImagePath))
                {
                    Directory.CreateDirectory(ImagePath);
                }
                FileInfo F = new FileInfo(File.FileName);
                ImageName = Guid.NewGuid().ToString() + F.Extension;
                string FullPath = Path.Combine(ImagePath, ImageName);
                using (var stream = new FileStream(FullPath, FileMode.Create))
                {
                    File.CopyTo(stream);
                }
            }
            return FilePath + "/" + ImageName;
        }
    }
}
