using System.ComponentModel.DataAnnotations;

namespace Traeq.DTO
{
    public class LoginDTO
    {

      
        [Required(ErrorMessage = "Username or Email is required")]
        [Display(Name = "Username / Email")]
        
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}