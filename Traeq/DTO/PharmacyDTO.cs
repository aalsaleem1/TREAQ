using Traeq.Models;

namespace Traeq.DTO
{
    public class PharmacyDTO : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public string? PharmacyName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? OwnerName { get; set; }
        public string? PharmacyLogoUrl { get; set; }
        public IFormFile? File { get; set; }
        public List<PharmacyLegalInfo>? PharmacyLegalsList { get; set; }
        public List<User>? UserList { get; set; }
    }

    public class PharmacyDetailsViewModel
    {
        public int Id { get; set; } 
        public string? PharmacyName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? OwnerName { get; set; }
        public string? PharmacyLogoUrl { get; set; }

        public double? Latitude { get; set; }   
        public double? Longitude{   get; set; }  


        public List<string>? Categories { get; set; }

        public List<PharmacyPromoCode>? PromoCodes { get; set; }
        public string? SelectedCategory { get; set; }

        public List<Medicine>? MedicinesList { get; set; }
    }
}