using Microsoft.AspNetCore.Identity;

namespace UserMicroservices.Data.Model.Entity
{
    public class User : IdentityUser
    {
        public UserStatus Status { get; set; } = UserStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
    public enum UserStatus
    {
        Active,
        Inactive
    }

}

