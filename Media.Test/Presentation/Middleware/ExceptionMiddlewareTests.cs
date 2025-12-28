using Media.Core.Exceptions;
using Media.Presentation.Middleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Test.Presentation.Middleware
{
    [TestClass]
    public class ExceptionMiddlewareTests
    {

        [TestMethod]
        public async Task HandleExceptionAsync_WritesStatus400BadRequest_WhenBadRequestException()
        {
            // Arrange.
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask);
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
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask);
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
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask);
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
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask);
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
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask);
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
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask);
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
            var middleware = new ExceptionMiddleware((_) => Task.CompletedTask);
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
