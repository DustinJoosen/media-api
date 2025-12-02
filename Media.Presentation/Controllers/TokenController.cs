using Media.Abstractions.Interfaces;
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
        public async Task<Guid> CreateToken() =>
            await this._tokenService.CreateToken();
    }
}
