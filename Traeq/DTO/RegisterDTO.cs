using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Traeq.DTO
{
    public class RegisterDTO : IValidatableObject
    {
        
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$",
        ErrorMessage = "Username can contain only letters and numbers")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        [MaxLength(30, ErrorMessage = "Username must be at most 30 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
        ErrorMessage = "Full name can contain only letters and spaces")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string? PhoneNumber { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }

        [Required]
        public string? AccountType { get; set; }

        public IFormFile? UserFile { get; set; }
        public string? ImageUrl { get; set; }

        public string? PharmacyName { get; set; }

        public string? LicenseNumber { get; set; }

        public string? OwnerName { get; set; }

        public IFormFile? LogoFile { get; set; }
        public string? PharmacyLogoUrl { get; set; }
        public double? Latitude { get; set; }       
        public double? Longitude { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.Equals(AccountType, "Pharmacy", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(PharmacyName))
                {
                    yield return new ValidationResult("Pharmacy name is required for pharmacy accounts", new[] { nameof(PharmacyName) });
                }

                if (string.IsNullOrWhiteSpace(LicenseNumber))
                {
                    yield return new ValidationResult("License number is required for pharmacy accounts", new[] { nameof(LicenseNumber) });
                }

                if (string.IsNullOrWhiteSpace(OwnerName))
                {
                    yield return new ValidationResult("Owner name is required for pharmacy accounts", new[] { nameof(OwnerName) });
                }
            }
        }
    }
}