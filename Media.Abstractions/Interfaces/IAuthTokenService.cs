using Media.Core.Dtos;

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
        /// Finds the info of an authorization token.
        /// </summary>
        /// <param name="token">token to find information of.</param>
        /// <returns>info of the token.</returns>
        Task<FindTokenInfoResponse> FindTokenInfo(string token);

        /// <summary>
        /// Deactivates an authorization token.
        /// </summary>
        /// <param name="token">token to deactivate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        Task DeactivateToken(string token);
    }
}
