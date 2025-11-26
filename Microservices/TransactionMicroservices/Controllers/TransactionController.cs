using Microsoft.AspNetCore.Mvc;

namespace TransactionMicroservices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase

    {
        [HttpGet]
        public IActionResult Transaction() { 
            return Ok("Transaction Microservice is working!");
        }
    }
}
