using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Traeq.Areas.Pharmacy.ViewModels;
using Traeq.Data;
using Traeq.Models;

namespace Traeq.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    [Authorize(Roles = "Pharmacy")]
    public class MedicinesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MedicinesController(
            AppDbContext context,
            UserManager<User> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<(User User, int? PharmacyId)> GetUserContextAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return (null, null);

            int? pharmacyId = null;

            if (user.PharmacyId != null)
            {
                pharmacyId = user.PharmacyId;
            }
            else
            {
                var pharmacy = await _context.PharmacyLegalInfos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.UserId == userId);
                pharmacyId = pharmacy?.Id;
            }

            return (user, pharmacyId);
        }

        public async Task<IActionResult> Index(string search)
        {
            var context = await GetUserContextAsync();
            if (context.PharmacyId == null) return RedirectToAction("Login", "User", new { area = "" });

            var medicinesQuery = _context.Medicines
                .Where(m => m.PharmacyLegalInfoId == context.PharmacyId && !m.IsDelete);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                medicinesQuery = medicinesQuery.Where(m =>
                    m.MedicineName.Contains(search) ||
                    m.ScientificName.Contains(search) ||
                    m.Category.Contains(search) ||
                    m.Id.ToString().Contains(search));
            }

            var medicines = await medicinesQuery
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return View(medicines);
        }


        public async Task<IActionResult> Create()
        {
            var context = await GetUserContextAsync();

            if (context.User.PharmacyId != null && !context.User.CanAddMedicine)
            {
                TempData["ErrorMessage"] = "You do not have permission to add medicines.";
                return RedirectToAction("Index", "Medicines", new { area = "Pharmacy" });
            }


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMedicineViewModel model)
        {
            var context = await GetUserContextAsync();
            if (context.PharmacyId == null) return RedirectToAction("Index");

            if (context.User.PharmacyId != null && !context.User.CanAddMedicine)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string? imagePath = null;

                    if (model.MedicineImage != null && model.MedicineImage.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/medicines");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.MedicineImage.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.MedicineImage.CopyToAsync(stream);
                        }

                        imagePath = $"images/medicines/{uniqueFileName}";
                    }

                    var medicine = new Medicine
                    {
                        MedicineName = model.MedicineName,
                        ScientificName = model.ScientificName,
                        Category = model.Category,
                        Price = model.Price,
                        Quantity = model.Quantity,
                        ExpiryDate = model.ExpiryDate,
                        MedicineDescription = model.MedicineDescription,
                        ImageURL = imagePath,
                        IsActive = true,
                        IsDelete = false,
                        PharmacyLegalInfoId = context.PharmacyId.Value,
                        CreateId = context.User.Id,
                        CreateDate = DateTime.Now
                    };

                    _context.Medicines.Add(medicine);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Medicine added successfully!";
                    return RedirectToAction("Index", "Medicines", new { area = "Pharmacy" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var context = await GetUserContextAsync();
            if (context.PharmacyId == null) return RedirectToAction("Index");

            if (context.User.PharmacyId != null && !context.User.CanEditMedicine)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit medicines.";
                return RedirectToAction("Index");
            }

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id && m.PharmacyLegalInfoId == context.PharmacyId);

            if (medicine == null) return NotFound();

            var model = new CreateMedicineViewModel
            {
                MedicineName = medicine.MedicineName,
                Price = medicine.Price,
                Quantity = medicine.Quantity,
                Category = medicine.Category,
                ScientificName = medicine.ScientificName,
                MedicineDescription = medicine.MedicineDescription,
                ExpiryDate = medicine.ExpiryDate,
                ImageURL = medicine.ImageURL
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateMedicineViewModel model)
        {
            var context = await GetUserContextAsync();
            if (context.PharmacyId == null) return RedirectToAction("Index");

            if (context.User.PharmacyId != null && !context.User.CanEditMedicine)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid) return View(model);

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id && m.PharmacyLegalInfoId == context.PharmacyId);

            if (medicine == null) return NotFound();

            string? imagePath = medicine.ImageURL;

            if (model.MedicineImage != null)
            {
                var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "images/medicines");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + "_" + model.MedicineImage.FileName;
                var path = Path.Combine(uploads, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await model.MedicineImage.CopyToAsync(stream);

                imagePath = $"images/medicines/{fileName}";
            }

            medicine.MedicineName = model.MedicineName;
            medicine.ScientificName = model.ScientificName;
            medicine.Category = model.Category;
            medicine.Price = model.Price;
            medicine.Quantity = model.Quantity;
            medicine.ExpiryDate = model.ExpiryDate;
            medicine.MedicineDescription = model.MedicineDescription;
            medicine.ImageURL = imagePath;
            medicine.EditId = context.User.Id;
            medicine.EditDate = DateTime.Now;

            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Medicines", new { area = "Pharmacy" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var context = await GetUserContextAsync();
            if (context.PharmacyId == null) return RedirectToAction("Index");

            if (context.User.PharmacyId != null && !context.User.CanDeleteMedicine)
            {
                TempData["ErrorMessage"] = "You do not have permission to delete medicines.";
                return RedirectToAction("Index");
            }

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id && m.PharmacyLegalInfoId == context.PharmacyId);

            if (medicine != null)
            {
                medicine.IsDelete = true;
                medicine.IsActive = false;
                _context.Medicines.Update(medicine);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Medicines", new { area = "Pharmacy" });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var context = await GetUserContextAsync();
            if (context.PharmacyId == null) return RedirectToAction("Index");

            if (context.User.PharmacyId != null && !context.User.CanEditMedicine)
            {
                return RedirectToAction("Index");
            }

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id && m.PharmacyLegalInfoId == context.PharmacyId);

            if (medicine != null)
            {
                medicine.IsActive = !medicine.IsActive;
                _context.Medicines.Update(medicine);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Medicines", new { area = "Pharmacy" });
        }
    }
}