using Microsoft.AspNetCore.Mvc;

namespace DataBaseWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        // Action for handling GET requests to "/"
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to the API!");
        }
    }
}
