using Microsoft.AspNetCore.Mvc;

namespace NotificationMicroservices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        [HttpGet]
        public IActionResult Notification()
        {

            return Ok("Notification Microservice is working!");
        }
    }
}
