using Media.Core.Dtos.Exchange;
using Media.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Media.Presentation.Controllers
{
    /// <summary>
    /// Health endpoint. Easy to trigger and check if the API is up and running.
    /// </summary>
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        public static DateTime AppRunningTime;
        private readonly IWebHostEnvironment _env;
        private readonly MediaDbContext _context;

        public HealthController(IWebHostEnvironment env, MediaDbContext context)
        {
            this._env = env;
            this._context = context;
        }

        /// <summary>
        /// Checks if the API is running fine.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<HealthResponse> Health()
        {
            // Runtime.
            var runtime = this.GetRuntimeInformation();
            
            // Dependencies.
            var dependencies = await this.GetDependenciesInformation();

            // Output formatting.
            return new HealthResponse(
                Status: "Ok",
                Version: "1.0.0",
                Runtime: runtime,
                Dependencies: dependencies);
        }

        /// <summary>
        /// Get the runtime information.
        /// </summary>
        /// <returns>All information about the runtime.</returns>
        private RuntimeResponse GetRuntimeInformation()
        {
            // Environment and configuration.
            var environment = this._env.EnvironmentName;

#if DEBUG
            string build = "Debug";
#elif RELEASE
            string build = "Release";
#endif

            // Uptime of the API.
            var startTime = AppRunningTime.ToString("yyyy-MM-dd HH:mm:ss");
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var uptimeSpan = DateTime.Now - AppRunningTime;
            var uptime = $"" +
				$"{(int)uptimeSpan.TotalHours}h " +
				$"{uptimeSpan.Minutes}m " +
				$"{uptimeSpan.Seconds}s";

            return new RuntimeResponse(environment, build, startTime, uptime, timestamp);
        }

        /// <summary>
        /// Get the dependency information.
        /// </summary>
        /// <returns>All information about the dependencies.</returns>
        private async Task<DependenciesResponse> GetDependenciesInformation()
        {
            try
            {
                await this._context.Database.OpenConnectionAsync();
                return new DependenciesResponse(Database: "Healthy");
            }
            catch
            {
                return new DependenciesResponse(Database: "Down");
            }
        }
    }
}
