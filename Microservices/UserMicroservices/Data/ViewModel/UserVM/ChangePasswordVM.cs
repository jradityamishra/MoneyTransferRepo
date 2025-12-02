using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Data.ViewModel.UserVM
{
    public class ChangePasswordVM
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
