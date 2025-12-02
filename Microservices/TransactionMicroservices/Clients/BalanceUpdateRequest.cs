namespace TransactionMicroservices.Clients
{
    public class BalanceUpdateRequest
    {
       public  string AccountNumber { get; set; }
        public int Amount { get; set; }
        public string operation { get; set; }
    }
}