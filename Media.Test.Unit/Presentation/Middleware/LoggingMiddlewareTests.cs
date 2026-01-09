using Media.Presentation.Middleware;
using Media.Test.Core.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Test.Unit.Presentation.Middleware
{
    [TestClass]
    public class LoggingMiddlewareTests
    {

        [TestMethod]
        public async Task InvokeAsync_Logs_IfHappyPath()
        {
            // Arrange.
            var nextCalled = false;
            var logger = new FakeLogger<LoggingMiddleware>();
            var middleware = new LoggingMiddleware((_) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            }, logger);

            // Act.
            await middleware.InvokeAsync(new DefaultHttpContext());

            // Assert.
            Assert.IsTrue(nextCalled);
            Assert.AreEqual(1, logger.Messages.Count);
        }
    }
}
