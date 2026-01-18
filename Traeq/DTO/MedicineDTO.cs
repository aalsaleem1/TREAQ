using System.ComponentModel.DataAnnotations;
using Traeq.Models;

namespace Traeq.DTO
{
    public class MedicineDTO : BaseEntity
    {
        [Required]
        public string? MedicineName { get; set; }


        [Required]
        public DateTime? ExpiryDate { get; set; }
        [Required]
        public string? MedicineDescription { get; set; }
        public string? ScientificName { get; set; }
        public string? ImageURL { get; set; }

        public IFormFile? File { get; set; }
        [Required]
        public int? Quantity { get; set; }
        [Required]
        public decimal? Price { get; set; }
        [Required]
        public string? Category { get; set; }
        [Required]
        public int PharmacyLegalInfoId { get; set; }

        public PharmacyLegalInfo? PharmacyLegalInfo { get; set; }

        public List<PharmacyLegalInfo>? PharmacyLegalInfoList { get; set; }
        public List<Medicine>? MedicineList { get; set; }

    }
}
