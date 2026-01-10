using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Media.Presentation.Attributes;
using Media.Core.Exceptions;

namespace Media.Test.Unit.Presentation.Attributes
{
    [TestClass]
    public class TokenRequiredAttributeTests
    {
        private static AuthorizationFilterContext CreateAuthorizationContext(string? authHeader = null)
        {
            var context = new DefaultHttpContext();

            if (authHeader != null)
                context.Request.Headers["Authorization"] = authHeader;

            var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
            return new AuthorizationFilterContext(actionContext, []);
        }

        [TestMethod]
        public void OnAuthorization_ThrowsUnauthorizedException_WhenTokenIsMissing()
        {
            // Arrange.
            var attribute = new TokenRequiredAttribute();
            var context = TokenRequiredAttributeTests.CreateAuthorizationContext();

            // Act / Assert.
            var ex = Assert.ThrowsException<UnauthorizedException>(() =>
            {
                attribute.OnAuthorization(context);
            });
        }

        [TestMethod]
        public void OnAuthorization_DoesNotThrow_WhenTokenIsProvided()
        {
            // Arrange.
            var attribute = new TokenRequiredAttribute();
            var context = TokenRequiredAttributeTests.CreateAuthorizationContext("Testing token");

            // Act.
            attribute.OnAuthorization(context);

            // Assert.
        }
    }
}
