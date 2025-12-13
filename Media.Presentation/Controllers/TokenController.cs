using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
using Media.Core.Exceptions;
using Media.Presentation.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Media.Presentation.Controllers
{
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
        public async Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq) =>
            await this._tokenService.CreateToken(tokenReq);

        /// <summary>
        /// Finds information about an authorization token; the Expiration date and whether the token is active.
        /// </summary>
        /// <returns>The expiration date + whether the token is active.</returns>
        [HttpGet]
        [Route("info")]
        [TokenRequired]
        public async Task<FindTokenInfoResponse> FindTokenInfo()
        {
            string token = this.Request.Headers.Authorization.ToString();
            return await this._tokenService.FindTokenInfo(token);
        }

        /// <summary>
        /// Deactivate an authorization token. It will remain in the database, but it can't be used any longer.
        /// </summary>
        [HttpDelete]
        [Route("deactivate-token")]

        public async Task<OkObjectResult> DeactivateToken()
        {
            string token = this.Request.Headers.Authorization.ToString();
            await this._tokenService.DeactivateToken(token);

            return this.Ok($"Token '{token}' successfully deactivated.");
        }
    }
}
