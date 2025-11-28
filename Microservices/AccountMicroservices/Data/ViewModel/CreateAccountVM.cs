using AccountMicroservices.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace AccountMicroservices.Data.ViewModel
{
    public class CreateAccountVM
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int Balance { get; set; } = 0;

        [Required]
        public string Currency { get; set; }

        public Account.AccountStatus Status { get; set; } = Account.AccountStatus.Active;

    }
}
