using TransactionMicroservices.Model.DTO;

namespace TransactionMicroservices.Model
{
    public class TransactionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TransactionDto Transaction { get; set; }
        public OperationDetails DebitOperation { get; set; }
        public OperationDetails CreditOperation { get; set; }
    }

    public class OperationDetails
    {
        public string AccountNumber { get; set; }
        public string OperationType { get; set; }
        public decimal Amount { get; set; }
        public int? PreviousBalance { get; set; }
        public int? NewBalance { get; set; }
        public string Status { get; set; }
    }
}
