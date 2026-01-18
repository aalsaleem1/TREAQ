using Traeq.Models;

namespace Traeq.DTO
{
    public class HomeDTO
    {
      
        public UserDTO? UserDTO { get; set; }
       
        public ContactUs? ContactUs { get; set; }
        


        public List<Cart>? CartList { get; set; }
        public List<Medicine>? MedicineList { get; set; }
        public List<PharmacyLegalInfo>? PharmacyLegalInfoList { get; set; }
    }
}
