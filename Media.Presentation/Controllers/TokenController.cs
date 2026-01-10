using Media.Abstractions.Interfaces;
using Media.Core.Dtos.Exchange;
using Media.Presentation.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Media.Presentation.Controllers
{
    /// <summary>
    /// Endpoints about the authorization keys.
    /// </summary>
    [Route("tokens")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAuthTokenService _tokenService;
        public TokenController(IAuthTokenService tokenService)
        {
            this._tokenService = tokenService;
        }

        /// <summary>
        /// Create an authorization token. The token can be used for uploading media.
        /// </summary>
        /// <returns>Created token.</returns>
        [HttpPost]
        [Route("create-token")]
        public async Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq, CancellationToken cancellationToken = default) =>
            await this._tokenService.CreateToken(tokenReq, cancellationToken);

        /// <summary>
        /// Finds information about an authorization token; the Expiration date and whether the token is active.
        /// </summary>
        /// <returns>The expiration date + whether the token is active.</returns>
        [HttpGet]
        [Route("info")]
        [TokenRequired]
        public async Task<FindTokenInfoResponse> FindTokenInfo(CancellationToken cancellationToken = default)
        {
            string token = this.Request.Headers.Authorization.ToString();
            return await this._tokenService.FindTokenInfo(token, cancellationToken);
        }

        /// <summary>
        /// Changes permissions of an authorization token. Note that this action requires a token with the CanManagePermissions permission.
        /// </summary>
        [HttpPut]
        [Route("change-permissions")]
        [TokenRequired]
        public async Task<IActionResult> ChangeTokenPermission(ChangeTokenPermissionRequest changeTokenPermissionReq, CancellationToken cancellationToken = default)
        {
            string token = this.Request.Headers.Authorization.ToString();
            await this._tokenService.ChangePermissions(changeTokenPermissionReq, token, cancellationToken);
            
            return this.Ok($"Token '{changeTokenPermissionReq.Token}' has been given new permissions.");
        }

        /// <summary>
        /// Deactivate an authorization token. It will remain in the database, but it can't be used any longer.
        /// </summary>
        [HttpDelete]
        [Route("deactivate-token")]
        public async Task<OkObjectResult> DeactivateToken(CancellationToken cancellationToken = default)
        {
            string token = this.Request.Headers.Authorization.ToString();
            await this._tokenService.DeactivateToken(token, cancellationToken);

            return this.Ok($"Token '{token}' successfully deactivated.");
        }
    }
}
