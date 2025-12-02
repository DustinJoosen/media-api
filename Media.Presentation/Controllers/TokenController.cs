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
        [Route("info")]
        public async Task<FindTokenInfoResponse> FindTokenInfo([FromQuery] FindTokenInfoRequest findTokenReq) =>
            await this._tokenService.FindTokenInfo(findTokenReq);

        [HttpDelete]
        [Route("deactivate-token")]
        public async Task<OkObjectResult> DeactivateToken(string token)
        {
            await this._tokenService.DeactivateToken(token);
            return this.Ok($"Token '{token}' successfully deactivated.");
        }
    }
}
