using System.ComponentModel.DataAnnotations;

namespace UserMicroservices.Data.ViewModel.UserVM
{
    public class AssignRoleVM
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
