using Media.Core.Exceptions;
using Media.Core.Options;
using Media.Presentation.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Media.Test.Unit.Presentation.Middleware
{
    [TestClass]
    public class RateLimitingMiddlewareTests
    {
        [TestMethod]
        public async Task InvokeAsync_CallsNext_UnderLimit()
        {
            // Arrange.
            var nextCalled = false;
            var middleware = new RateLimitingMiddleware((_) => 
            {
                nextCalled = true;
                return Task.CompletedTask;
            }, 
            new MemoryCache(new MemoryCacheOptions()), Options.Create(new RateLimitingOptions(3, 60)));
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            
            // Act.
            await middleware.InvokeAsync(context);

            // Assert.
            Assert.IsTrue(nextCalled);
        }

        [TestMethod]
        public async Task InvokeAsync_ThrowsTooManyRequestsException_WhenOverLimit()
        {
            // Arrange.
            var middleware = new RateLimitingMiddleware((_) => Task.CompletedTask, 
                new MemoryCache(new MemoryCacheOptions()), Options.Create(new RateLimitingOptions(1, 60)));
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            await middleware.InvokeAsync(context);

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<TooManyRequestsException>(async () =>
                await middleware.InvokeAsync(context));

            // Assert.
            Assert.AreEqual(0, ex?.Remaining);
            Assert.AreEqual(1, ex?.Limit);
        }


        [TestMethod]
        public async Task InvokeAsync_CounterResetAndCallsNext_ResetsCounter()
        {
            // Arrange.
            var nextCalled = false;
            var middleware = new RateLimitingMiddleware((_) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            new MemoryCache(new MemoryCacheOptions()), Options.Create(new RateLimitingOptions(1, 1)));
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Loopback;
            await middleware.InvokeAsync(context);

            // Act. 
            await Task.Delay(1100);
            await middleware.InvokeAsync(context);

            // Assert.
            Assert.IsTrue(nextCalled);
        }
    }
}
