using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Media.Presentation.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Checks if the API is running fine.
        /// </summary>
        [HttpGet]
        [Route("")]
        public IActionResult Health() =>
            this.Ok("Media API is running");
    }
}
