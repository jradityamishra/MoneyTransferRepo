using AccountMicroservices.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace AccountMicroservices.Data.ViewModel
{
    public class UpdateAccountVM
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int Balance { get; set; }

        [Required]
        public string Currency { get; set; }

        public string Status { get; set; }

    }
}
