using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class PharmacySupportMessage
    {
        [Key]
        public int Id { get; set; }

        public int ThreadId { get; set; }
        [ForeignKey("ThreadId")]
        public PharmacySupportThread Thread { get; set; } = default!;

        [Required]
        public string SenderType { get; set; } = "User";

        [Required]
        public string MessageText { get; set; } = default!;

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
