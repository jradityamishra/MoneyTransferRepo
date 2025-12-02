namespace AccountMicroservices.Data.ViewModel
{
    public class BalanceUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UpdatedBalance { get; set; }
    }
}
