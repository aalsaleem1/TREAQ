using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using Traeq.Data;
using Traeq.DTO;
using Traeq.Models;

namespace Traeq.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public CheckoutController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private (decimal subtotal, decimal discount, decimal total) CalculateTotals(List<Cart> cartItems)
        {
            decimal subtotal = cartItems.Sum(c => c.TotalPrice);
            decimal discount = 0m;

            if (HttpContext.Session.TryGetValue("PromoDiscount", out var bytes))
            {
                var str = Encoding.UTF8.GetString(bytes);
                decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out discount);
            }

            if (discount > subtotal) discount = subtotal;
            decimal total = subtotal - discount;
            if (total < 0) total = 0;

            return (subtotal, discount, total);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.Carts
                .Include(c => c.Medecine)
                .Where(c => c.UserId == userId && !c.IsCheckedOut)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Cart", new { area = "" });
            }

            var addresses = await _context.UserAddresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var totals = CalculateTotals(cartItems);

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                Subtotal = totals.subtotal,
                DiscountAmount = totals.discount,
                TotalAmount = totals.total,
                ExistingAddresses = addresses ?? new List<UserAddress>()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.Carts
                .Include(c => c.Medecine)
                .Where(c => c.UserId == userId && !c.IsCheckedOut)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Cart", new { area = "" });
            }

            ModelState.Remove("CartItems");
            ModelState.Remove("ExistingAddresses");

            if (model.DeliveryMethod == "Delivery")
            {
                if (model.SelectedAddressId == null && (string.IsNullOrEmpty(model.NewCity) || string.IsNullOrEmpty(model.NewFullAddress)))
                {
                    ModelState.AddModelError("", "Please select an address or enter a new one.");
                }
            }

            var totals = CalculateTotals(cartItems);

            if (!ModelState.IsValid)
            {
                model.CartItems = cartItems;
                model.Subtotal = totals.subtotal;
                model.DiscountAmount = totals.discount;
                model.TotalAmount = totals.total;
                model.ExistingAddresses = await _context.UserAddresses.Where(a => a.UserId == userId).ToListAsync();
                return View(model);
            }

            string finalCity = "", finalDistrict = "", finalAddress = "", finalPhone = "";
            double? finalLat = null, finalLng = null;

            if (model.DeliveryMethod == "Delivery")
            {
                if (model.SelectedAddressId != null)
                {
                    var existingAddress = await _context.UserAddresses.FindAsync(model.SelectedAddressId);
                    if (existingAddress != null)
                    {
                        finalCity = existingAddress.City;
                        finalDistrict = existingAddress.District;
                        finalAddress = existingAddress.FullAddress;
                        finalPhone = existingAddress.PhoneNumber;
                        finalLat = existingAddress.Latitude;
                        finalLng = existingAddress.Longitude;
                    }
                }
                else
                {
                    var newAddress = new UserAddress
                    {
                        UserId = userId,
                        City = model.NewCity,
                        District = model.NewDistrict,
                        FullAddress = model.NewFullAddress,
                        PhoneNumber = model.NewPhoneNumber,
                        Latitude = model.NewLatitude,
                        Longitude = model.NewLongitude
                    };
                    _context.UserAddresses.Add(newAddress);
                    await _context.SaveChangesAsync();
                    finalCity = model.NewCity;
                    finalDistrict = model.NewDistrict;
                    finalAddress = model.NewFullAddress;
                    finalPhone = model.NewPhoneNumber;
                    finalLat = model.NewLatitude;
                    finalLng = model.NewLongitude;
                }
            }

            var order = new Order
            {
                UserId = userId,
                PharmacyId = cartItems.First().PharmacyId ?? 0,
                OrderDate = DateTime.Now,
                CancelAllowedUntil = DateTime.Now.AddMinutes(10),
                Subtotal = totals.subtotal,
                DiscountAmount = totals.discount,
                TotalAmount = totals.total,
                OrderStatus = "Pending",
                PaymentMethod = model.PaymentMethod,
                IsPaid = false,
                DeliveryMethod = model.DeliveryMethod,
                ShippingCity = finalCity,
                ShippingDistrict = finalDistrict,
                ShippingAddressDetails = finalAddress,
                ShippingPhoneNumber = finalPhone,
                ShippingLatitude = finalLat,
                ShippingLongitude = finalLng
            };


            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    MedicineId = item.MedicineId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            _context.Carts.RemoveRange(cartItems);
            HttpContext.Session.Remove("PromoDiscount");
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", "Checkout", new { id = order.Id });
        }

        [HttpGet]
        [Route("Checkout/OrderConfirmation/{id}")]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Pharmacy)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Medicine)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
