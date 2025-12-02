namespace TransactionMicroservices.Model
{
    public class BalanceUpdateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? Balance { get; set; }
    }
}
