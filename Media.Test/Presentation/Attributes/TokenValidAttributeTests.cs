using Media.Abstractions.Interfaces;
using Media.Infrastructure.Services;
using Media.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Media.Core.Exceptions;
using Media.Presentation.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Media.Test.Presentation.Attributes
{
    [TestClass]
    public class TokenValidAttributeTests
    {
        private MediaDbContext _context;
        private IAuthTokenService _service;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MediaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            this._context = new MediaDbContext(options);
            this._service = new AuthTokenService(this._context);

            this._context.AuthTokens.Add(new Core.Entities.AuthToken
            {
                Name = "Testing Auth Token Valid",
                Token = "VALID_TOKEN",
                Permissions = (Core.Entities.AuthTokenPermissions)31,
                IsActive = true
            });
            this._context.AuthTokens.Add(new Core.Entities.AuthToken
            {
                Name = "Testing Auth Token Expired",
                Token = "EXPIRED_TOKEN",
                Permissions = (Core.Entities.AuthTokenPermissions)31,
                IsActive = true,
                ExpiresAt = DateTime.Now.AddDays(-31)
            });
            this._context.AuthTokens.Add(new Core.Entities.AuthToken
            {
                Name = "Testing Auth Token Inactive",
                Token = "INACTIVE_TOKEN",
                Permissions = (Core.Entities.AuthTokenPermissions)31,
                IsActive = false
            });
            this._context.SaveChanges();
        }

        private static AuthorizationFilterContext CreateAuthorizationContext(IAuthTokenService service, string? authHeader = null)
        {
            var context = new DefaultHttpContext();

            if (authHeader != null)
                context.Request.Headers["Authorization"] = authHeader;

            var services = new ServiceCollection();
            services.AddSingleton<IAuthTokenService>(service);
            context.RequestServices = services.BuildServiceProvider();

            var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
            return new AuthorizationFilterContext(actionContext, []);
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_DoesNotThrow_WhenValidTokenIsProvided()
        {
            // Arrange.
            var attribute = new TokenValidAttribute();
            var context = TokenValidAttributeTests.CreateAuthorizationContext(this._service, "VALID_TOKEN");

            // Act.
            await attribute.OnAuthorizationAsync(context);

            // Assert.
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_ThrowsUnauthorizedException_WhenTokenIsMissing()
        {
            // Arrange.
            var attribute = new TokenValidAttribute();
            var context = TokenValidAttributeTests.CreateAuthorizationContext(this._service);

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
            {
                await attribute.OnAuthorizationAsync(context);
            });

            // Assert.
            Assert.AreEqual("Didn't provide an authorization token in header.", ex.Message);
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_ThrowsUnauthorizedException_WhenTokenIsInActive()
        {
            // Arrange.
            var attribute = new TokenValidAttribute();
            var context = TokenValidAttributeTests.CreateAuthorizationContext(this._service, "INACTIVE_TOKEN");

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
            {
                await attribute.OnAuthorizationAsync(context);
            });

            // Assert.
            Assert.AreEqual("Could not use this token. Provided token is deactivated.", ex.Message);
        }

        
        [TestMethod]
        public async Task OnAuthorizationAsync_ThrowsUnauthorizedException_WhenTokenIsExpired()
        {
            // Arrange.
            var attribute = new TokenValidAttribute();
            var context = TokenValidAttributeTests.CreateAuthorizationContext(this._service, "EXPIRED_TOKEN");

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
            {
                await attribute.OnAuthorizationAsync(context);
            });

            // Assert.
            Assert.AreEqual("Could not use this token. Provided token is expired.", ex.Message);
        }



    }
}
