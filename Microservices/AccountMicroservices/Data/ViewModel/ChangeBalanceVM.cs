using System.ComponentModel.DataAnnotations;

namespace AccountMicroservices.Data.ViewModel
{
    public class ChangeBalanceVM
    {
            [Required]
            public string AccountNumber { get; set; }

            [Required]
            public int Amount { get; set; }
        
    }
}
