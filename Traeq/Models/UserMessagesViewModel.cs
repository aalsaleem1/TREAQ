using System.Collections.Generic;

namespace Traeq.Models
{
    public class UserMessagesViewModel
    {
        public List<OrderSupportThread> AdminThreads { get; set; } = new();
        public List<PharmacySupportThread> PharmacyThreads { get; set; } = new();
    }
}
