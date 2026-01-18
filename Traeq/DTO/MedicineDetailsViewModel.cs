namespace Traeq.DTO
{
    public class MedicineDetailsViewModel
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public string ScientificName { get; set; }
        public string MedicineDescription { get; set; }
        public string ImageURL { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Category { get; set; }
    }
}
