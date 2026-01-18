using System.ComponentModel.DataAnnotations;
using Traeq.Models;

namespace Traeq.DTO
{
    public class CheckoutViewModel
    {
        public List<Cart> CartItems { get; set; } = new List<Cart>();

        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public List<UserAddress> ExistingAddresses { get; set; } = new List<UserAddress>();

        public int? SelectedAddressId { get; set; }

        public string? NewCity { get; set; }
        public string? NewDistrict { get; set; }
        public string? NewFullAddress { get; set; }
        public string? NewPhoneNumber { get; set; }
        public double? NewLatitude { get; set; }
        public double? NewLongitude { get; set; }

        [Required(ErrorMessage = "Payment Method is required")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Delivery Method is required")]
        public string DeliveryMethod { get; set; }
    }
}
