using Microsoft.AspNetCore.Mvc;

namespace AccountMicroservices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        [HttpGet("test")]
      public IActionResult Account()
        {
            return Ok("Account Microservice is working!");
        }   
    }
}
