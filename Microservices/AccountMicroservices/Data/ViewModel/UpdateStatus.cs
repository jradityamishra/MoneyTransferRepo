using System.ComponentModel.DataAnnotations;

namespace AccountMicroservices.Data.ViewModel
{
    public class UpdateStatus
    {

        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string Status { get; set; } = "Active";
    }
}
