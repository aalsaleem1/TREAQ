using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string City { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string FullAddress { get; set; } 
        public string? PhoneNumber { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}