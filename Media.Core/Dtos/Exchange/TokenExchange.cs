using Media.Core.Entities;

namespace Media.Core.Dtos.Exchange
{
    /// <summary>
    /// Input for token creation.
    /// </summary>
    /// <param name="Name">Name of the token.</param>
    /// <param name="ExpiresAt">Datetime when the token expires. Optional.</param>
    public record CreateTokenRequest(string Name, DateTime? ExpiresAt);

    /// <summary>
    /// Output for token creation.
    /// </summary>
    /// <param name="Token">Created token object.</param>
    public record CreateTokenResponse(string Token);

    /// <summary>
    /// Output for finding token info.
    /// </summary>
    /// <param name="Name">Name of the token.</param>
    /// <param name="ExpiresAt">Expiration date.</param>
    /// <param name="IsActive">If the token is active.</param>
    /// <param name="Permissions">Permissions of the token.</param>
    public record FindTokenInfoResponse(
        string Name, 
        DateTime? ExpiresAt, 
        bool IsActive, 
        AuthTokenPermissions Permissions);

    /// <summary>
    /// Input for changing token permissions.
    /// </summary>
    /// <param name="Token">Token to change the permissions of.</param>
    /// <param name="Permission">New permissions.</param>
    public record ChangeTokenPermissionRequest(string Token, AuthTokenPermissions Permission);
}
