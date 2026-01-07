using Media.Core.Exceptions;
using Media.Presentation.Middleware;
using Media.Test.Unit.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Test.Unit.Presentation.Middleware
{

    [TestClass]
    public class ExceptionMiddlewareTests
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddlewareTests()
        {
            this._env = new FakeWebHostEnvironment();
            this._logger = NullLogger<ExceptionMiddleware>.Instance;
        }


        [TestMethod]
        public async Task HandleExceptionAsync_WritesStatus400BadRequest_WhenBadRequestException()
        {
            // Arrange.
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask, this._env, this._logger);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new BadRequestException("Bad Request!");

            // Act.
            await middleware.HandleExceptionAsync(context, exception);

            // Assert.
            Assert.AreEqual(400, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleExceptionAsync_WritesStatus401Unauthorized_WhenUnauthorizedException()
        {
            // Arrange.
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask, this._env, this._logger);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new UnauthorizedException("Unauthorized!");

            // Act.
            await middleware.HandleExceptionAsync(context, exception);

            // Assert.
            Assert.AreEqual(401, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleExceptionAsync_WritesStatus404NotFound_WhenNotFoundException()
        {
            // Arrange.
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask, this._env, this._logger);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new NotFoundException("Not Found!");

            // Act.
            await middleware.HandleExceptionAsync(context, exception);

            // Assert.
            Assert.AreEqual(404, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleExceptionAsync_WritesStatus409Conflict_WhenAlreadyUsedException()
        {
            // Arrange.
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask, this._env, this._logger);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new AlreadyUsedException("Already used!");

            // Act.
            await middleware.HandleExceptionAsync(context, exception);

            // Assert.
            Assert.AreEqual(409, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleExceptionAsync_WritesStatus429TooManyRequests_WhenTooManyRequestsException()
        {
            // Arrange.
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask, this._env, this._logger);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new TooManyRequestsException("Too many requests! Get rate limited!", 50, 0, 7);

            // Act.
            await middleware.HandleExceptionAsync(context, exception);

            // Assert.
            Assert.AreEqual(429, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleExceptionAsync_WritesStatus500InternalServerError_WhenDatabaseOperationException()
        {
            // Arrange.
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask, this._env, this._logger);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new DatabaseOperationException("Database Operation!");

            // Act.
            await middleware.HandleExceptionAsync(context, exception);

            // Assert.
            Assert.AreEqual(500, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleExceptionAsync_WritesStatus500InternalServerError_WhenOtherException()
        {
            // Arrange.
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask, this._env, this._logger);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var exception = new Exception("Database Operation!");

            // Act.
            await middleware.HandleExceptionAsync(context, exception);

            // Assert.
            Assert.AreEqual(500, context.Response.StatusCode);
        }


    }
}
