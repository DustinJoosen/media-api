namespace Media.Core.Exceptions
{
    public class TooManyRequestsException : CustomException
    {
        public int Limit { get; set; }
        public int Remaining {get; set;}
        public int RetryAfterSeconds { get; set; }

        public TooManyRequestsException(string message, int limit, int remaining, int retryAfterSeconds) : base(message)
        {
            this.Limit = limit;
            this.Remaining = remaining;
            this.RetryAfterSeconds = retryAfterSeconds;
        }

        public override Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>
            {
                ["X-RateLimit-Limit"] = this.Limit.ToString(),
                ["X-RateLimit-Remaining"] = this.Remaining.ToString(),
                ["X-RateLimit-Reset"] = this.RetryAfterSeconds.ToString()
            };
        }
    }
}
