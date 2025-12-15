using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Core.Dtos
{
    /// <summary>
    /// Output for a health checkup.
    /// </summary>
    /// <param name="Status">"Ok".</param>
    /// <param name="Environment">Development or Production.</param>
    /// <param name="Configuration">Debug or Release.</param>
    /// <param name="Uptime">Running time of the api.</param>
    /// <param name="StartTime">Starting timestamp of when the api is running.</param>
    /// <param name="Database">Database information.</param>
    public record HealthResponse(
        string Status,
        string Environment,
        string Configuration,
        string Uptime,
        string StartTime,
        HealthDatabaseResponse Database);

    /// <summary>
    /// Output about the database.
    /// </summary>
    /// <param name="IsRunning">Is the database running.</param>
    /// <param name="Server">What server is database on.</param>
    /// <param name="Version">Version of the database.</param>
    public record HealthDatabaseResponse(
        bool IsRunning,
        string Server,
        string Version);
}
