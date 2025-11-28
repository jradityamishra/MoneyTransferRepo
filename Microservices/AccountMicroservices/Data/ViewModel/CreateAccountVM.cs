using AccountMicroservices.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace AccountMicroservices.Data.ViewModel
{
    public class CreateAccountVM
    {
        public string UserID { get; set; }

        public string AccountNumber { get; set; }

      
        public int Balance { get; set; } = 0;

        public string Currency { get; set; }

        public string Status { get; set; }

    }
}
