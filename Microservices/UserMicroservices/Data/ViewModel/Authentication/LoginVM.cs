using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Data.ViewModel.Authentication
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
