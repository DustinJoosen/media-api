using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Core.Options
{
	public class RateLimitingOptions
    {
        public int RequestsPerWindow { get; set; }
        public int WindowSeconds { get; set; }

        public RateLimitingOptions() { }

        public RateLimitingOptions(int requestsPerWindow, int windowSeconds)
        {
            this.RequestsPerWindow = requestsPerWindow;
            this.WindowSeconds = windowSeconds;
        }
    }
}
