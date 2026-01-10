using Media.Core.Exceptions;
using Media.Core.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Media.Presentation.Middleware
{
    /// <summary>
    /// Middleware that ensures users don't go over their rate limits.
    /// It keeps track of how often a certain device has made a request, and if it goes 
    /// over the given limit, it will block further requests until the time window expires.
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly RateLimitingOptions _options;

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, 
            IOptions<RateLimitingOptions> options)
        {
            this._next = next;
            this._cache = cache;
            this._options = options.Value;

            // To offset the request from the initial loading.
            this._options.RequestsPerWindow++;
        }

        /// <summary>
        /// This method is called when any request is made. 
        /// It adds the user's IP to a cache set, and counts how often it's made a request.
        /// If the request goes over the limit, it throws a TooManyRequest exception.
        /// When the time window expires, the counter is reset back to 0.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            var key = context.Connection?.RemoteIpAddress?.ToString() ?? "unkown";

            // Try to find the existing window and counter. If it doesn't exist, make it.
            bool cacheContains = this._cache.TryGetValue(key, out (DateTime, int) value);
            if (cacheContains)
                this._cache.Set(key, (value.Item1, value.Item2 + 1));

            // Reset if the window has expired, or the cache doesn't exist yet.
            if (!cacheContains || (value.Item1 + TimeSpan.FromSeconds(this._options.WindowSeconds)) <= DateTime.UtcNow)
                this._cache.Set(key, (DateTime.UtcNow, 1));

            // If the counter is higher or equal to the limit, throw a 429.
            var (start, counter) = this._cache.Get<(DateTime, int)>(key);
            if (counter >= this._options.RequestsPerWindow)
            {
                var retry = (int)(start + TimeSpan.FromSeconds(this._options.WindowSeconds) - DateTime.UtcNow).TotalSeconds;
                throw new TooManyRequestsException(
                    message: $"You’re sending requests too quickly. Please wait {retry} seconds before trying again.",
                    limit: this._options.RequestsPerWindow - 1,
                    remaining: 0,
                    retryAfterSeconds: retry
                );
            }

            // Continue ^^.
            await this._next(context);
        }
    }
}
