using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Traeq.Data;
using Traeq.Models;

namespace Traeq.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    [Authorize(Roles = "Pharmacy")]
    public class PromoCodesController : Controller
    {
        private readonly AppDbContext _context;

        public PromoCodesController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<PharmacyLegalInfo?> GetCurrentPharmacyAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            if (user.PharmacyId != null)
            {
                return await _context.PharmacyLegalInfos
                    .FirstOrDefaultAsync(p => p.Id == user.PharmacyId);
            }

            return await _context.PharmacyLegalInfos
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IActionResult> Index()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Index", "Dashboard");

            var list = await _context.PharmacyPromoCodes
                .Where(p => p.PharmacyId == pharmacy.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Index", "Dashboard");

            var now = DateTime.Now;
            var model = new PharmacyPromoCode
            {
                StartDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0),
                EndDateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddDays(7),
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PharmacyPromoCode model)
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Index", "Dashboard");

            if (model.StartDateTime != default && model.EndDateTime != default &&
                model.EndDateTime <= model.StartDateTime)
            {
                ModelState.AddModelError(string.Empty, "End date/time must be after start date/time.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.PharmacyId = pharmacy.Id;
            model.CreatedAt = DateTime.Now;

            _context.PharmacyPromoCodes.Add(model);
            await _context.SaveChangesAsync();

            TempData["PromoSuccess"] = "The Promo Code Has Been Successfully Added..";

            return RedirectToAction(
                actionName: "Index",
                controllerName: "PromoCodes",
                routeValues: new { area = "Pharmacy" });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Index", "Dashboard");

            var promo = await _context.PharmacyPromoCodes
                .FirstOrDefaultAsync(p => p.Id == id && p.PharmacyId == pharmacy.Id);

            if (promo == null) return NotFound();

            return View(promo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PharmacyPromoCode model)
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Index", "Dashboard");

            if (id != model.Id) return NotFound();

            if (model.EndDateTime <= model.StartDateTime)
            {
                ModelState.AddModelError("", "End date/time must be after start date/time.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var promo = await _context.PharmacyPromoCodes
                .FirstOrDefaultAsync(p => p.Id == id && p.PharmacyId == pharmacy.Id);

            if (promo == null) return NotFound();

            promo.Code = model.Code;
            promo.DiscountPercent = model.DiscountPercent;
            promo.StartDateTime = model.StartDateTime;
            promo.EndDateTime = model.EndDateTime;
            promo.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["PromoSuccess"] = "The promo code has been successfully modified.";

            return RedirectToAction(
                actionName: "Index",
                controllerName: "PromoCodes",
                routeValues: new { area = "Pharmacy" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var pharmacy = await GetCurrentPharmacyAsync();
            if (pharmacy == null) return RedirectToAction("Index", "Dashboard");

            var promo = await _context.PharmacyPromoCodes
                .FirstOrDefaultAsync(p => p.Id == id && p.PharmacyId == pharmacy.Id);

            if (promo != null)
            {
                _context.PharmacyPromoCodes.Remove(promo);
                await _context.SaveChangesAsync();
            }

            TempData["PromoSuccess"] = "The promo code was successfully deleted.";

            return RedirectToAction(
                actionName: "Index",
                controllerName: "PromoCodes",
                routeValues: new { area = "Pharmacy" });
        }

    }
}
