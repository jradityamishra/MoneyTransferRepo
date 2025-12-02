namespace TransactionMicroservices.Model.Enums
{
    public enum TransactionStatus
    {
        Initiated,
        Pending,
        Processing,
        Completed,
        Failed,
        Cancelled,
        Reversed
    }
}
