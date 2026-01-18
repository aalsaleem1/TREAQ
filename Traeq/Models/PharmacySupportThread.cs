using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class PharmacySupportThread
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;
        [ForeignKey("UserId")]
        public User User { get; set; } = default!;

        public int PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public PharmacyLegalInfo Pharmacy { get; set; } = default!;

        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public bool IsClosed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ClosedAt { get; set; }

        public ICollection<PharmacySupportMessage> Messages { get; set; } = new List<PharmacySupportMessage>();
    }
}
