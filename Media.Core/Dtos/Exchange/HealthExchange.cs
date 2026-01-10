namespace Media.Core.Dtos.Exchange
{
    /// <summary>
    /// Output for a health checkup.
    /// </summary>
    /// <param name="Status">"Ok".</param>
    /// <param name="Version">Current version of the project.</param>
    /// <param name="Runtime">Runtime information.</param>
    /// <param name="Dependencies">Dependencies informatino.</param>
    public record HealthResponse(string Status, string Version, RuntimeResponse Runtime, DependenciesResponse Dependencies);

    /// <summary>
    /// Output for the runtime information.
    /// </summary>
    /// <param name="Environment">Development or Production.</param>
    /// <param name="Build">Debug or Release.</param>
    /// <param name="Uptime">Running time of the api.</param>
    /// <param name="StartTime">Starting timestamp of when the api is running.</param>
    public record RuntimeResponse(string Environment, string Build, string StartTime, string Uptime, string Timestamp);

    /// <summary>
    /// Output about the dependencies.
    /// </summary>
    /// <param name="Database">Is the database running</param>
    public record DependenciesResponse(string Database);
}
