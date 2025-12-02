using Media.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Abstractions.Interfaces
{
    public interface IAuthTokenService
    {
        /// <summary>
        /// Create an authorization token.
        /// </summary>
        /// <param name="tokenReq">token creation information.</param>
        /// <returns>Created token.</returns>
        Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq);
        
        /// <summary>
        /// Finds the expiration date of an authorization token.
        /// </summary>
        /// <param name="findTokenReq">token finding information.</param>
        /// <returns>Expiration date of token.</returns>
        Task<FindTokenExpirationResponse> FindTokenExpiration(FindTokenExpirationRequest findTokenReq);

        /// <summary>
        /// Resets an authorization token.
        /// </summary>
        /// <param name="token">token to reset.</param>
        /// <returns>Whether the operation succeeded.</returns>
        Task ResetToken(string token);
    }
}
