using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class Cart : BaseEntity
    {

        [Required]
        public String? UserId { get; set; }

        public virtual User? User { get; set; }

        [Required]
        public int MedicineId { get; set; }

        public virtual Medicine? Medecine { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        public int? PharmacyId { get; set; }

        public virtual PharmacyLegalInfo? Pharmacy { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public bool IsCheckedOut { get; set; } = false;

        public string? OrderId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
    }
}
