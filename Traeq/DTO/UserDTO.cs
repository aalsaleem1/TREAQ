using System.ComponentModel.DataAnnotations;

namespace Traeq.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? ImageUrl { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "District")]
        public string? District { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Pharmacy Name")]
        public string? PharmacyName { get; set; }

        [Display(Name = "Pharmacy License")]
        public string? PharmacyLicense { get; set; }

        [Display(Name = "Owner Name")]
        public string? OwnerName { get; set; }
        [Display(Name = "Pharmacy Logo")]
        public string? PharmacyLogoUrl { get; set; }

        public IFormFile? UserFile { get; set; }
        public IFormFile? LogoFile { get; set; }
    }
}