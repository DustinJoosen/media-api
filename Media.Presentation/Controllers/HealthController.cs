using Media.Core.Dtos;
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
        private static readonly DateTime AppRunningTime = DateTime.UtcNow;
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
            // Environment and configuration.
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var configuration = environment == "Development" ? "Debug" : "Release";

            // Uptime of the API.
            var uptime = (DateTime.UtcNow - AppRunningTime).ToString(@"dd\.hh\:mm\:ss");
            var startTime = AppRunningTime.ToString("yyyy-MM-dd HH:mm:ss");

            // Database connection.
            var databaseInformation = await this.GetDatabaseInformation();

            // Output formatting.
            return new HealthResponse(
                Status: "Ok",
                Environment: environment,
                Configuration: configuration,
                Uptime: uptime,
                StartTime: startTime,
                Database: databaseInformation);
        }


        /// <summary>
        /// Get the database health information.
        /// </summary>
        /// <returns>Database server information.</returns>
        private async Task<HealthDatabaseResponse> GetDatabaseInformation()
        {
            try
            {
                await this._context.Database.OpenConnectionAsync();
                var connection = this._context.Database.GetDbConnection();

                return new HealthDatabaseResponse(
                    IsRunning: true,
                    Server: connection.DataSource,
                    Version: connection.ServerVersion);
            }
            catch
            {
                return new HealthDatabaseResponse(
                    IsRunning: false,
                    Server: "N/A",
                    Version: "N/A");
            }
        }
    }
}
