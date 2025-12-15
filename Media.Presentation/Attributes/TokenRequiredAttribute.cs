using Media.Abstractions.Interfaces;
using Media.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Media.Presentation.Attributes
{
    /// <summary>
    /// Add the attribute [TokenRequired] above a method and it will require an authtoken.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TokenRequiredAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// Checks if a token is present.
        /// </summary>
        /// <param name="context">Authorization context.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedException("Didn't provide an authorization token in header.");
        }
    }
}
