using System.ComponentModel.DataAnnotations;
namespace TransactionMicroservices.Model.DTO
{
    public class TransactionRequest
    {
        [Required]
        public string FromAccountId { get; set; }
        [Required]
        public string? ToAccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string Description { get; set; } = null;
    }
}
