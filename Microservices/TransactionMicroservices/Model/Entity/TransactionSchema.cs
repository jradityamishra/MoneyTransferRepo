using TransactionMicroservices.Model.Enums;
namespace TransactionMicroservices.Model.Entity
{
    public class TransactionSchema
    {
        public Guid Id { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
    }

   
}
