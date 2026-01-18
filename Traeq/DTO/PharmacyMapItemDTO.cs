namespace Traeq.DTO
{
    public class PharmacyMapItemDTO
    {
        public int Id { get; set; }
        public string? PharmacyName { get; set; }
        public string? OwnerName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? City { get; set; }
        public string? DetailsUrl { get; set; }
    }
}
