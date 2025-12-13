using Media.Abstractions.Interfaces;
using Media.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Media.Presentation.Attributes
{
    /// <summary>
    /// Add the attribute [TokenRequired] above a method and it will require a valid authtoken.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TokenRequiredAttribute : Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// Checks if a token is present, active, and not yet expired.
        /// </summary>
        /// <param name="context">Authorization context.</param>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedException("Didn't provide an authorization token in header.");

            var service = context.HttpContext.RequestServices.GetRequiredService<IAuthTokenService>();
            var tokenInfo = await service.FindTokenInfo(token);

            if (!tokenInfo.IsActive)
                throw new UnauthorizedException("Could not upload this media item. Provided token is deactivated.");

            if (tokenInfo.ExpiresAt < DateTime.Now)
                throw new UnauthorizedException("Could not upload this media item. Provided token is expired.");
        }
    }
}
