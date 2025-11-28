using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Data.ViewModel.UserVM
{
    public class CreateUserVM
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // optional
        public string UserName { get; set; }

        // initial role(s)
        public string[] Roles { get; set; } = new string[0];
    }
}
