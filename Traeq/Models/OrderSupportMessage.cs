using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class OrderSupportMessage
    {
        [Key]
        public int Id { get; set; }

        public int ThreadId { get; set; }
        [ForeignKey("ThreadId")]
        public OrderSupportThread Thread { get; set; }

        [Required]
        public string SenderType { get; set; } = "User";

        [Required]
        public string MessageText { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
