using Traeq.Models;

namespace Traeq.DTO
{
    public class SearchViewModel
    {
        public string SearchQuery { get; set; }
        public List<Medicine> Medicines { get; set; }
        public List<PharmacyLegalInfo> Pharmacies { get; set; }
    }
}