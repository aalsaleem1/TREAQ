using System.ComponentModel.DataAnnotations;

namespace Traeq.Models
{
    public class Medicine : BaseEntity
    {
        [Required]
        public string? MedicineName { get; set; }


        [Required]
        public DateTime? ExpiryDate { get; set; }
        [Required]
        public string? MedicineDescription { get; set; }
        public string? ScientificName { get; set; }
        public string? ImageURL { get; set; }
        [Required]
        public int? Quantity { get; set; }
        [Required]
        public decimal? Price { get; set; }
        [Required]
        public string? Category { get; set; }
        [Required]
        public int PharmacyLegalInfoId { get; set; }

        public PharmacyLegalInfo? PharmacyLegalInfo { get; set; }

    }
}
