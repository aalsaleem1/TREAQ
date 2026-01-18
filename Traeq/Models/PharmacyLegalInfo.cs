namespace Traeq.Models
{
    public class PharmacyLegalInfo : BaseEntity
    {


        public string? UserId { get; set; }
        public User? User { get; set; }

        public string? PharmacyName { get; set; }

        public string? LicenseNumber { get; set; }


        public string? OwnerName { get; set; }

        public string? PharmacyLogoUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
