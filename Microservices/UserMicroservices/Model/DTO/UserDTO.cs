namespace UserMicroservices.Model.DTO
{
    public class UserDTO
    {

        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string Roles { get; set; }
    }
}
