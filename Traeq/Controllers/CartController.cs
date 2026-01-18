using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Traeq.Data;
using Traeq.Models;

namespace Traeq.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
       
        public CartController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.Carts
                .Include(c => c.Medecine)
                .Include(c => c.Pharmacy)
                .Where(c => c.UserId == userId && !c.IsCheckedOut)
                .ToListAsync();

            decimal discount = 0m;

            if (HttpContext.Session.TryGetValue("PromoDiscount", out var bytes))
            {
                var str = Encoding.UTF8.GetString(bytes);
                decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out discount);
            }

            ViewBag.DiscountAmount = discount;

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int medicineId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            var medicine = await _context.Medicines.FindAsync(medicineId);

            if (medicine == null) return NotFound();

            var existingCartPharmacyId = await _context.Carts
                .Where(c => c.UserId == userId && !c.IsCheckedOut)
                .Select(c => c.PharmacyId)
                .FirstOrDefaultAsync();

            if (existingCartPharmacyId != null && existingCartPharmacyId != 0 && existingCartPharmacyId != medicine.PharmacyLegalInfoId)
            {
                var oldItems = await _context.Carts
                    .Where(c => c.UserId == userId && !c.IsCheckedOut)
                    .ToListAsync();
                _context.Carts.RemoveRange(oldItems);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("PromoDiscount");
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.MedicineId == medicineId && !c.IsCheckedOut);

            var stock = medicine.Quantity ?? 0;

            if (existingCartItem != null)
            {
                var newQuantity = existingCartItem.Quantity + quantity;

                if (stock > 0 && newQuantity > stock)
                {
                    newQuantity = stock;
                }

                existingCartItem.Quantity = newQuantity;
                existingCartItem.TotalPrice = existingCartItem.Quantity * existingCartItem.UnitPrice;
                existingCartItem.CreatedDate = DateTime.Now;
            }
            else
            {
                if (stock > 0 && quantity > stock)
                {
                    quantity = stock;
                }

                var cartItem = new Cart
                {
                    UserId = userId,
                    MedicineId = medicineId,
                    Quantity = quantity,
                    PharmacyId = medicine.PharmacyLegalInfoId,
                    UnitPrice = medicine.Price ?? 0,
                    TotalPrice = (medicine.Price ?? 0) * quantity,
                    IsCheckedOut = false,
                    CreatedDate = DateTime.Now
                };
                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            var cartItem = await _context.Carts
                .Include(c => c.Medecine)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cartItem != null && quantity > 0)
            {
                var stock = cartItem.Medecine?.Quantity ?? 0;

                if (stock > 0 && quantity > stock)
                {
                    quantity = stock;
                }

                cartItem.Quantity = quantity;
                cartItem.TotalPrice = cartItem.Quantity * cartItem.UnitPrice;
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("PromoDiscount");
            }
            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("PromoDiscount");
            }
            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _context.Carts.Where(c => c.UserId == userId && !c.IsCheckedOut).ToListAsync();
            _context.Carts.RemoveRange(items);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("PromoDiscount");

            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyPromo(string promoCode)
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.Carts
                .Include(c => c.Medecine)
                .Include(c => c.Pharmacy)
                .Where(c => c.UserId == userId && !c.IsCheckedOut)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["PromoError"] = "Cart is empty.";
                HttpContext.Session.Remove("PromoDiscount");
                return RedirectToAction("Index", "Cart", new { area = "" });
            }

            if (string.IsNullOrWhiteSpace(promoCode))
            {
                TempData["PromoError"] = "Please enter promo code.";
                HttpContext.Session.Remove("PromoDiscount");
                return RedirectToAction("Index", "Cart", new { area = "" });
            }

            var pharmacyId = cartItems.First().PharmacyId;
            if (pharmacyId == null || pharmacyId == 0)
            {
                TempData["PromoError"] = "Cannot detect pharmacy for this cart.";
                HttpContext.Session.Remove("PromoDiscount");
                return RedirectToAction("Index", "Cart", new { area = "" });
            }

            var now = DateTime.Now;

            var promo = await _context.PharmacyPromoCodes
                .FirstOrDefaultAsync(p =>
                    p.PharmacyId == pharmacyId &&
                    p.Code == promoCode &&
                    p.IsActive &&
                    now >= p.StartDateTime &&
                    now <= p.EndDateTime);

            if (promo == null)
            {
                TempData["PromoError"] = "Promo code is invalid or expired.";
                HttpContext.Session.Remove("PromoDiscount");
                return RedirectToAction("Index", "Cart", new { area = "" });
            }

            var subtotal = cartItems.Sum(c => c.TotalPrice);
            var discountAmount = subtotal * promo.DiscountPercent / 100m;

            var discountString = discountAmount.ToString(CultureInfo.InvariantCulture);
            HttpContext.Session.Set("PromoDiscount", Encoding.UTF8.GetBytes(discountString));

            TempData["PromoSuccess"] = $"Promo applied: {promo.DiscountPercent}% discount.";

            return RedirectToAction("Index", "Cart", new { area = "" });
        }
    }
}
