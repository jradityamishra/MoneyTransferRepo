namespace UserMicroservices.Model.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string HashPassword { get; set; }
        public string Roles { get; set; } = string.Empty;
    }
}
