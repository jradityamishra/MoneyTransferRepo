using Microsoft.AspNetCore.Identity;

namespace UserMicroservices.Data.Model.Entity
{
    public class User : IdentityUser
    {
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
 

}

