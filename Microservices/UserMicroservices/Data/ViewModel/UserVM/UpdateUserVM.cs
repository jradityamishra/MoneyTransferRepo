using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Data.ViewModel.UserVM
{
    public class UpdateUserVM
    {
        [EmailAddress]
        public string Email { get; set; }

        public string UserName { get; set; }

        public bool? IsActive { get; set; }
    }
}
