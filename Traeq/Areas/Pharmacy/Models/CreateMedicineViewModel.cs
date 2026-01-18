using System.ComponentModel.DataAnnotations;

namespace Traeq.Areas.Pharmacy.ViewModels
{
    public class CreateMedicineViewModel
    {
        [Required(ErrorMessage = "Medicine Name is required")]
        [Display(Name = "Medicine Name")]
        public string? MedicineName { get; set; }

        [Required(ErrorMessage = "Expiry Date is required")]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Required(ErrorMessage = "Medicine Description is required")]
        [Display(Name = "Medicine Description")]
        public string? MedicineDescription { get; set; }

        [Display(Name = "Scientific Name")]
        public string? ScientificName { get; set; }

        [Display(Name = "Medicine Image")]
        public IFormFile? MedicineImage { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Display(Name = "Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int? Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        public string? ImageURL { get; set; }
    }
}