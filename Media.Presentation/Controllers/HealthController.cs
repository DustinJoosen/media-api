using Microsoft.AspNetCore.Mvc;

namespace Media.Presentation.Controllers
{
    /// <summary>
    /// Health endpoint. Easy to trigger and check if the API is up and running.
    /// </summary>
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
