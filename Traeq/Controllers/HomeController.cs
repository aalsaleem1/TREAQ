using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Traeq.Data;
using Traeq.DTO;
using Traeq.Models;
using Traeq.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Traeq.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public IRepository<Medicine> medicine { get; set; }
        public IRepository<PharmacyLegalInfo> pharmacy { get; set; }
        public IRepository<Cart> cart { get; set; }
        public AppDbContext _context { get; set; }

        public HomeController(
            ILogger<HomeController> logger,
            IRepository<Medicine> medicine,
            IRepository<PharmacyLegalInfo> pharmacy,
            IRepository<Cart> cart,
            AppDbContext context,
            UserManager<User> userManager
            )
        {
            _logger = logger;
            this.medicine = medicine;
            this.pharmacy = pharmacy;
            this.cart = cart;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var pharmacyList = _context.PharmacyLegalInfos
                .Include(p => p.User)
                .Where(p => !p.IsDelete && p.IsActive)
                .ToList();

            var data = new HomeDTO
            {
                MedicineList = medicine?.View() ?? new List<Medicine>(),
                PharmacyLegalInfoList = pharmacyList
            };

            return View(data);
        }

        public async Task<IActionResult> ShopNowRedirect()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User", new { area = "" });
            }

            var user = await _userManager.GetUserAsync(User);

            if (user != null && user.AccountType == "User")
            {
                return Redirect("/Home/Index#pharmacies-section");
            }

            return RedirectToAction("Index");
        }

        public IActionResult Stor()
        {
            var medicinesWithPharmacy = _context.Medicines
                .Include(m => m.PharmacyLegalInfo)
                .Where(m => !m.IsDelete && m.IsActive)
                .ToList();

            var data = new HomeDTO
            {
                MedicineList = medicinesWithPharmacy
            };
            return View(data);
        }

        public IActionResult AboutUs()
        {
            var data = new HomeDTO
            {
                MedicineList = medicine?.View() ?? new List<Medicine>(),
            };
            return View(data);
        }

        public IActionResult MedicineDetails(int id)
        {
            var Medicine = medicine.Find(id);
            if (Medicine == null || Medicine.IsDelete || !Medicine.IsActive)
                return NotFound();

            var model = new MedicineDetailsViewModel
            {
                Id = Medicine.Id,
                MedicineName = Medicine.MedicineName,
                ScientificName = Medicine.ScientificName,
                MedicineDescription = Medicine.MedicineDescription,
                ImageURL = Medicine.ImageURL,
                Price = Medicine.Price ?? 0,
                Quantity = Medicine.Quantity ?? 0,
                ExpiryDate = Medicine.ExpiryDate ?? DateTime.MinValue,
                Category = Medicine.Category
            };
            return View(model);
        }

        public async Task<IActionResult> PharmacyDetails(int id)
        {
            var Pharmacy = await _context.PharmacyLegalInfos.FindAsync(id);
            if (Pharmacy == null) return NotFound();

            var fixedCategories = new List<string>
    {
        "Antibiotics", "Painkillers", "Anti-inflammatory",
        "Vitamins", "Chronic Diseases", "Supplements", "Other"
    };

            var promos = await _context.PharmacyPromoCodes
                .Where(p => p.PharmacyId == Pharmacy.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var model = new PharmacyDetailsViewModel
            {
                Id = Pharmacy.Id,
                LicenseNumber = Pharmacy.LicenseNumber,
                OwnerName = Pharmacy.OwnerName,
                PharmacyLogoUrl = Pharmacy.PharmacyLogoUrl,
                PharmacyName = Pharmacy.PharmacyName,
                Latitude = Pharmacy.Latitude,
                Longitude = Pharmacy.Longitude,
                Categories = fixedCategories,
                MedicinesList = new List<Medicine>(),
                PromoCodes = promos
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult Search(string query)
        {
            var model = new SearchViewModel { SearchQuery = query, Medicines = new List<Medicine>(), Pharmacies = new List<PharmacyLegalInfo>() };
            if (!string.IsNullOrEmpty(query))
            {
                model.Medicines = _context.Medicines
                    .Include(m => m.PharmacyLegalInfo)
                    .Where(m => (m.MedicineName.Contains(query) || m.ScientificName.Contains(query)) && !m.IsDelete && m.IsActive)
                    .ToList();

                model.Pharmacies = _context.PharmacyLegalInfos
                    .Include(p => p.User)
                    .Where(p => p.PharmacyName.Contains(query) && !p.IsDelete && p.IsActive)
                    .ToList();
            }
            return View(model);
        }

        private List<string> GetCategorySearchTerms(string category)
        {
            var terms = new List<string> { category };
            switch (category)
            {
                case "Antibiotics": terms.Add("مضادات حيوية"); break;
                case "Painkillers": terms.Add("مسكنات"); break;
                case "Anti-inflammatory": terms.Add("مضادات التهاب"); break;
                case "Vitamins": terms.Add("فيتامينات"); break;
                case "Chronic Diseases": terms.Add("أدوية مزمنة"); break;
                case "Supplements": terms.Add("مكملات غذائية"); break;
                case "Other": terms.Add("أخرى"); break;
            }
            return terms;
        }

        public IActionResult PharmacyMedicines(int pharmacyId, string category)
        {
            var Pharmacy = pharmacy.Find(pharmacyId);
            if (Pharmacy == null) return NotFound();
            var searchTerms = GetCategorySearchTerms(category);
            var medicines = _context.Medicines
                .Where(m => m.PharmacyLegalInfoId == pharmacyId && searchTerms.Contains(m.Category) && !m.IsDelete && m.IsActive)
                .ToList();

            var model = new PharmacyDetailsViewModel
            {
                Id = Pharmacy.Id,
                PharmacyName = Pharmacy.PharmacyName,
                PharmacyLogoUrl = Pharmacy.PharmacyLogoUrl,
                SelectedCategory = category,
                MedicinesList = medicines
            };
            return View(model);
        }

        public IActionResult PharmaciesMap()
        {
            var pharmacies = _context.PharmacyLegalInfos
                .Include(p => p.User)
                .Where(p => p.Latitude.HasValue && p.Longitude.HasValue)
                .ToList();

            var vm = pharmacies.Select(p => new PharmacyMapItemDTO
            {
                Id = p.Id,
                PharmacyName = p.PharmacyName,
                OwnerName = p.OwnerName,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                City = p.User != null ? p.User.City : null,
                DetailsUrl = Url.Action("PharmacyDetails", "Home", new { area = "", id = p.Id })

            }).ToList();

            return View(vm);
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(
            string supportType,
            int? orderId,
            string? nameFirst,
            string? nameLast,
            string? email,
            string? phone,
            string? subject,
            string? message,
            string? pharmacySupportMode,
            int? pharmacyId)
        {
            if (supportType == "site")
            {
                if (string.IsNullOrWhiteSpace(nameFirst) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(message))
                {
                    TempData["ContactError"] = "Please fill in the required fields.";
                    return View();
                }

                var contact = new ContactUs
                {
                    NameFirst = nameFirst,
                    NameLast = nameLast,
                    Email = email,
                    Subject = subject,
                    Message = $"Phone: {phone ?? "N/A"}\n\n{message}",
                    CreateDate = DateTime.Now
                };

                _context.ContactUss.Add(contact);
                await _context.SaveChangesAsync();

                TempData["ContactSuccess"] = "Your message has been sent to our support team. We will contact you soon.";
                return View();
            }

            if (supportType == "order")
            {
                if (!User.Identity.IsAuthenticated)
                {
                    TempData["ContactError"] = "Please login to request order support.";
                    return View();
                }

                if (orderId == null)
                {
                    TempData["ContactError"] = "Please enter your order number.";
                    return View();
                }

                var userId = _userManager.GetUserId(User);
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if (order == null)
                {
                    TempData["ContactError"] = "Order not found. Please make sure the order number is correct.";
                    return View();
                }

                var existingThread = await _context.OrderSupportThreads
                    .FirstOrDefaultAsync(t => t.OrderId == order.Id && t.UserId == userId && !t.IsClosed);

                if (existingThread == null)
                {
                    existingThread = new OrderSupportThread
                    {
                        OrderId = order.Id,
                        UserId = userId,
                        CreatedAt = DateTime.Now,
                        IsClosed = false
                    };

                    _context.OrderSupportThreads.Add(existingThread);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("OrderSupportChat", "Home", new { area = "", threadId = existingThread.Id });
            }

            if (supportType == "pharmacy")
            {
                if (!User.Identity.IsAuthenticated)
                {
                    TempData["ContactError"] = "Please login to contact a pharmacy.";
                    return View();
                }

                var userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(pharmacySupportMode))
                {
                    TempData["ContactError"] = "Please choose pharmacy support type.";
                    return View();
                }

                Order? order = null;
                int targetPharmacyId;

                if (pharmacySupportMode == "order")
                {
                    if (orderId == null)
                    {
                        TempData["ContactError"] = "Please enter your order number.";
                        return View();
                    }

                    order = await _context.Orders
                        .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                    if (order == null)
                    {
                        TempData["ContactError"] = "Order not found. Please make sure the order number is correct.";
                        return View();
                    }

                    targetPharmacyId = order.PharmacyId;
                }
                else
                {
                    if (pharmacyId == null)
                    {
                        TempData["ContactError"] = "Please select a pharmacy.";
                        return View();
                    }

                    var pharmacyExists = await _context.PharmacyLegalInfos
                        .AnyAsync(p => p.Id == pharmacyId && !p.IsDelete && p.IsActive);

                    if (!pharmacyExists)
                    {
                        TempData["ContactError"] = "Selected pharmacy not found.";
                        return View();
                    }

                    targetPharmacyId = pharmacyId.Value;
                }

                PharmacySupportThread? existingThread;

                if (order != null)
                {
                    var orderIdValue = order.Id;

                    existingThread = await _context.PharmacySupportThreads
                        .FirstOrDefaultAsync(t =>
                            t.UserId == userId &&
                            t.PharmacyId == targetPharmacyId &&
                            t.OrderId == orderIdValue &&
                            !t.IsClosed);
                }
                else
                {
                    existingThread = await _context.PharmacySupportThreads
                        .FirstOrDefaultAsync(t =>
                            t.UserId == userId &&
                            t.PharmacyId == targetPharmacyId &&
                            t.OrderId == null &&
                            !t.IsClosed);
                }

                if (existingThread == null)
                {
                    existingThread = new PharmacySupportThread
                    {
                        UserId = userId,
                        PharmacyId = targetPharmacyId,
                        OrderId = order?.Id,
                        CreatedAt = DateTime.Now,
                        IsClosed = false
                    };

                    _context.PharmacySupportThreads.Add(existingThread);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("PharmacySupportChat", "Home", new { area = "", threadId = existingThread.Id });
            }


            TempData["ContactError"] = "Please select a support type.";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetPharmaciesForSupport()
        {
            var list = await _context.PharmacyLegalInfos
                .Where(p => !p.IsDelete && p.IsActive)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.PharmacyName
                })
                .ToListAsync();

            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> OrderSupportChat(int threadId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "User");

            var userId = _userManager.GetUserId(User);

            var thread = await _context.OrderSupportThreads
                .Include(t => t.Order)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t => t.Id == threadId && t.UserId == userId);

            if (thread == null)
                return NotFound();

            var messages = thread.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => new SupportChatMessageViewModel
                {
                    SenderType = m.SenderType,
                    SenderLabel = m.SenderType == "User" ? "You" : "Admin",
                    MessageText = m.MessageText,
                    SentAt = m.SentAt
                })
                .ToList();

            var vm = new SupportChatViewModel
            {
                ThreadId = thread.Id,
                Title = $"Order Support - #{thread.OrderId}",
                SubTitle = $"Order #: {thread.OrderId}",
                IsClosed = thread.IsClosed,
                SendAction = "SendOrderSupportMessage",
                CloseAction = "CloseOrderSupportThread",
                Messages = messages
            };

            return View("SupportChat", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOrderSupportMessage(int threadId, string messageText)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = _userManager.GetUserId(User);

            var thread = await _context.OrderSupportThreads
                .Include(t => t.Order)
                .FirstOrDefaultAsync(t => t.Id == threadId && t.UserId == userId && !t.IsClosed);

            if (thread == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(messageText))
            {
                return RedirectToAction("OrderSupportChat", "Home", new { area = "", threadId });
            }

            var msg = new OrderSupportMessage
            {
                ThreadId = thread.Id,
                SenderType = "User",
                MessageText = messageText,
                SentAt = DateTime.Now
            };

            _context.OrderSupportMessages.Add(msg);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderSupportChat", "Home", new { area = "", threadId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseOrderSupportThread(int threadId)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = _userManager.GetUserId(User);

            var thread = await _context.OrderSupportThreads
                .FirstOrDefaultAsync(t => t.Id == threadId && t.UserId == userId && !t.IsClosed);

            if (thread == null)
                return NotFound();

            thread.IsClosed = true;
            thread.ClosedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction("ContactUs", "Home", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> PharmacySupportChat(int threadId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "User");

            var userId = _userManager.GetUserId(User);

            var thread = await _context.PharmacySupportThreads
                .Include(t => t.Pharmacy)
                .Include(t => t.Order)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t => t.Id == threadId && t.UserId == userId);

            if (thread == null)
                return NotFound();

            var title = $"Pharmacy Support - {thread.Pharmacy.PharmacyName}";
            if (thread.OrderId.HasValue)
                title = $"Pharmacy Support - Order #{thread.OrderId} - {thread.Pharmacy.PharmacyName}";

            var subTitle = $"Pharmacy: {thread.Pharmacy.PharmacyName}";
            if (thread.OrderId.HasValue)
                subTitle += $" | Order #: {thread.OrderId}";

            var messages = thread.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => new SupportChatMessageViewModel
                {
                    SenderType = m.SenderType,
                    SenderLabel = m.SenderType == "User" ? "You" : "Pharmacy",
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
                SendAction = "SendPharmacySupportMessage",
                CloseAction = "ClosePharmacySupportThread",
                Messages = messages
            };

            return View("SupportChat", vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendPharmacySupportMessage(int threadId, string messageText)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = _userManager.GetUserId(User);

            var thread = await _context.PharmacySupportThreads
                .FirstOrDefaultAsync(t => t.Id == threadId && !t.IsClosed);

            if (thread == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(messageText))
            {
                return RedirectToAction("Chat", "Dashboard", new { area = "Pharmacy", threadId });
            }

            var msg = new PharmacySupportMessage
            {
                ThreadId = thread.Id,
                SenderType = userId == thread.UserId ? "User" : "Pharmacy",
                MessageText = messageText,
                SentAt = DateTime.Now
            };

            _context.PharmacySupportMessages.Add(msg);
            await _context.SaveChangesAsync();

            if (User.IsInRole("Pharmacy"))
                return RedirectToAction("Chat", "Dashboard", new { area = "Pharmacy", threadId });

            return RedirectToAction("PharmacySupportChat", "Home", new { area = "", threadId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClosePharmacySupportThread(int threadId)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var thread = await _context.PharmacySupportThreads
                .FirstOrDefaultAsync(t => t.Id == threadId && !t.IsClosed);

            if (thread == null)
                return NotFound();

            thread.IsClosed = true;
            thread.ClosedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            if (User.IsInRole("Pharmacy"))
                return RedirectToAction("MyMessages", "Dashboard", new { area = "Pharmacy" });

            return RedirectToAction("ContactUs", "Home", new { area = "" });
        }


        [HttpGet]
        public async Task<IActionResult> MyMessages()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "User");

            var userId = _userManager.GetUserId(User);

            var adminThreads = await _context.OrderSupportThreads
                .Include(t => t.Order)
                .Include(t => t.Messages)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var pharmacyThreads = await _context.PharmacySupportThreads
                .Include(t => t.Pharmacy)
                .Include(t => t.Order)
                .Include(t => t.Messages)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var vm = new UserMessagesViewModel
            {
                AdminThreads = adminThreads,
                PharmacyThreads = pharmacyThreads
            };

            return View(vm);
        }

    }
}
