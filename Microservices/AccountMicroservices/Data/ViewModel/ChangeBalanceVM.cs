using System.ComponentModel.DataAnnotations;

namespace AccountMicroservices.Data.ViewModel
{
    public class ChangeBalanceVM
    {
            
          

            [Required]
            public int Amount { get; set; }

        public string operation { get; set; }
        
    }
}