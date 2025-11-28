using System.Reflection.Metadata.Ecma335;

namespace UserMicroservices.Data.ViewModel.Authentication
{
    public class AuthResultVM
    {
        public string Token { get; set; }
        public string  RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        
    }
}
