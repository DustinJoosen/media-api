using Media.Core.Dtos.Exchange;
using Media.Core.Entities;

namespace Media.Abstractions.Interfaces
{
    public interface IAuthTokenService
    {
        /// <summary>
        /// Create an authorization token.
        /// </summary>
        /// <param name="tokenReq">Token creation information.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>Created token.</returns>
        Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds the info of an authorization token.
        /// </summary>
        /// <param name="token">Token to find information of.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>Info of the token.</returns>
        Task<FindTokenInfoResponse> FindTokenInfo(string token, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates an authorization token.
        /// </summary>
        /// <param name="token">Token to deactivate.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        Task DeactivateToken(string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the permissions an auth token has.
        /// </summary>
        /// <param name="token">Token to find permissions from.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>Permissions object of the given token.</returns>
        Task<AuthTokenPermissions> GetRoles(string token, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes permissions of an authorization token. Note that this action requires 
        /// a token with the CanManagePermissions permission.
        /// </summary>
        /// <param name="changeReq">The token to change, and the new permissions.</param>
        /// <param name="token">
        /// Token used to authorize the permission change (not the token being modified).
        /// </param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        Task ChangePermissions(ChangeTokenPermissionRequest changeReq, string token,
            CancellationToken cancellationToken = default);
    }
}
