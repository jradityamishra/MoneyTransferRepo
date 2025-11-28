using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Data.ViewModel.UserVM
{
    public class ResetPasswordVM
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
