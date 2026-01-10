using Media.Abstractions.Interfaces;
using Media.Core;
using Media.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Media.Presentation.Attributes
{
	/// <summary>
	/// Add the attribute [TokenValid] above a method and it will require a valid authtoken.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TokenValidAttribute : Attribute, IAsyncAuthorizationFilter
	{
		/// <summary>
		/// Checks if a token is present, active, and not yet expired.
		/// </summary>
		/// <param name="context">Authorization context.</param>
		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			var token = context.HttpContext.Request.Headers.Authorization.ToString();
			if (string.IsNullOrEmpty(token))
				throw new UnauthorizedException(ErrorMessages.NoAuthTokenInHeader());

			var service = context.HttpContext.RequestServices
				.GetRequiredService<IAuthTokenService>();
			var tokenInfo = await service.FindTokenInfo(token);

			if (!tokenInfo.IsActive)
				throw new UnauthorizedException(ErrorMessages.CannotUseTokenItIs("deactivated"));

			if (tokenInfo.ExpiresAt < DateTime.Now)
				throw new UnauthorizedException(ErrorMessages.CannotUseTokenItIs("expired"));
		}
	}
}
