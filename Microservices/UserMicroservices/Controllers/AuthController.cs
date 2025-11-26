using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserMicroservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpGet]
        public IActionResult TestEndpoint()
        {
            return Ok("User Microservice is working!");
        }
    }
}
