using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public PharmacyLegalInfo Pharmacy { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime CancelAllowedUntil { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = "Pending";

        public string PaymentMethod { get; set; } = string.Empty;
        public bool IsPaid { get; set; } = false;

        public string DeliveryMethod { get; set; } = string.Empty;

        public string? ShippingCity { get; set; }
        public string? ShippingDistrict { get; set; }
        public string? ShippingAddressDetails { get; set; }
        public string? ShippingPhoneNumber { get; set; }

        public string? RejectionReason { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
        public double? ShippingLatitude { get; set; }
        public double? ShippingLongitude { get; set; }

        public bool IsCanceled { get; set; } = false;
    }
}
