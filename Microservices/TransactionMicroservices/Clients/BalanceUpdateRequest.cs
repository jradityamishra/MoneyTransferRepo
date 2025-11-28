namespace TransactionMicroservices.Clients
{
    internal class BalanceUpdateRequest
    {
        public decimal Amount { get; set; }
        public string Operation { get; set; }
    }
}