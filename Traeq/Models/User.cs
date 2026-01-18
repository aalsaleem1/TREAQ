using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traeq.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Full UserName Required")]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        public string? AccountType { get; set; } 
        public string? City { get; set; }
        public string? District { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }


        public int? PharmacyId { get; set; }

        [ForeignKey("PharmacyId")]
        public virtual PharmacyLegalInfo? EmployedAtPharmacy { get; set; }

        public bool CanAddMedicine { get; set; }
        public bool CanEditMedicine { get; set; }
        public bool CanDeleteMedicine { get; set; }
        public bool CanViewDashboard { get; set; } = true; 


        public string? CreateId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? EditId { get; set; }
        public DateTime? EditDate { get; set; }

        public virtual ICollection<PharmacyLegalInfo>? PharmacyLegalInfo { get; set; }
    }
}