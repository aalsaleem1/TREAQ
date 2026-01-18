using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Traeq.Models
{
    public class PharmacyPromoCode
    {
        public int Id { get; set; }

        [Required]
        public int PharmacyId { get; set; }

        [ValidateNever]
        public PharmacyLegalInfo? Pharmacy { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; } = null!;

        [Range(1, 100, ErrorMessage = "Discount must be between 1 and 100")]
        public int DiscountPercent { get; set; }

        [Required(ErrorMessage = "Start date/time is required")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "End date/time is required")]
        public DateTime EndDateTime { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
