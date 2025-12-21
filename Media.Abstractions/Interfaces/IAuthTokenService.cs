using Media.Core.Dtos;
using Media.Core.Entities;

namespace Media.Abstractions.Interfaces
{
    public interface IAuthTokenService
    {
        /// <summary>
        /// Create an authorization token.
        /// </summary>
        /// <param name="tokenReq">Token creation information.</param>
        /// <returns>Created token.</returns>
        Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq);
        
        /// <summary>
        /// Finds the info of an authorization token.
        /// </summary>
        /// <param name="token">Token to find information of.</param>
        /// <returns>Info of the token.</returns>
        Task<FindTokenInfoResponse> FindTokenInfo(string token);

        /// <summary>
        /// Deactivates an authorization token.
        /// </summary>
        /// <param name="token">Token to deactivate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        Task DeactivateToken(string token);

        /// <summary>
        /// Gets the permissions an auth token has.
        /// </summary>
        /// <param name="token">Token to find permissions from.</param>
        /// <returns>Permissions object of the given token.</returns>
        Task<AuthTokenPermissions> GetRoles(string token);

        /// <summary>
        /// Changes permissions of an authorization token. Note that this action requires a token with the CanManagePermissions permission.
        /// </summary>
        /// <param name="changeTokenPermissionReq">The token to change, and the new permissions.</param>
        /// <param name="token">Token to proof the user is allowed to change permissions. NOT THE TOKEN THAT WILL BE CHANGED.</param>
        Task ChangePermissions(ChangeTokenPermissionRequest changeTokenPermissionReq, string token);
    }
}
