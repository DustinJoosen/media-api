using System.Text;

namespace Media.Presentation.Middleware
{
    /// <summary>
    /// Middleware that logs all HTTP requests that are made. It saves the timestamp, method, path,
    /// Auth key, body (if applicable).
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            this._next = next;
            this._logger = logger;
        }

        /// <summary>
        /// Logs the request that is made. Log the important information through the default ILogger.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var timestamp = DateTime.UtcNow;
            var method = context.Request.Method;
            var path = context.Request.Path + context.Request.QueryString;
            var authHeader = context.Request.Headers.ContainsKey("Authorization")
                ? context.Request.Headers["Authorization"].ToString()
                : null;

            string body = string.Empty;
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true);

                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            this._logger.LogInformation("Request {@Timestamp}: {@Method} {@Path}, Auth: {@Auth}, Body: {@Body}",
                timestamp, method, path, authHeader ?? "none", body != null ? this.Truncate(body, 200) : "empty");

            await this._next(context);
        }

        /// <summary>
        /// Truncates a string with 3 elippses. If a string as more then <paramref name="value"/> 
        /// characters, then replace the leftovers with the 3 elippses.
        /// </summary>
        /// <param name="value">Text to truncate.</param>
        /// <param name="maxLength">Length to cut off.</param>
        /// <returns>Truncated value.</returns>
        private string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) 
                return value;

            return value.Length <= maxLength
                ? value
                : value.Substring(0, maxLength) + "...";
        }
    }
}
