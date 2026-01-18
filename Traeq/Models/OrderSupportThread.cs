using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class OrderSupportThread
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public string? AdminId { get; set; }
        [ForeignKey("AdminId")]
        public User? Admin { get; set; }

        public bool IsClosed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ClosedAt { get; set; }

        public ICollection<OrderSupportMessage> Messages { get; set; } = new List<OrderSupportMessage>();
    }
}
