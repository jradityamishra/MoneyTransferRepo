using Azure.Identity;

namespace UserMicroservices.Data.ViewModel
{
    public class UserUpdateVM
    {
        public string UserName { get; set; }
        public string status { get; set; }
        public int Email { get; set; }
        
    }
}
