using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
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

        [HttpPost]
        [Route("create-token")]
        public async Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq) =>
            await this._tokenService.CreateToken(tokenReq);

        [HttpGet]
        [Route("find-expiration")]
        public async Task<FindTokenExpirationResponse> FindTokenExpiration([FromQuery] FindTokenExpirationRequest findTokenReq) =>
            await this._tokenService.FindTokenExpiration(findTokenReq);

        [HttpDelete]
        [Route("reset-token")]
        public async Task<OkObjectResult> ResetToken(string token)
        {
            await this._tokenService.ResetToken(token);
            return this.Ok($"Token '{token}' successfully reset.");
        }
    }
}
