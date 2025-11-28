namespace AccountMicroservices.Data.Model
{
    public class Account
    {

        
            public int Id { get; set; }
            public string UserID { get; set; }
            public string AccountNumber { get; set; }
            public int Balance { get; set; }
            public string Currency { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        

        
    }
}
