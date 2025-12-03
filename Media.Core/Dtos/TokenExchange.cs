using Media.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Core.Dtos
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
    /// Input for finding token info.
    /// </summary>
    /// <param name="Token">Token to find.</param>
    public record FindTokenInfoRequest(string Token);

    /// <summary>
    /// Output for finding token info.
    /// </summary>
    /// <param name="ExpiresAt">Expiration date.</param>
    /// <param name="IsActive">If the token is active.</param>
    /// <param name="Permissions">Permissions of the token.</param>
    public record FindTokenInfoResponse(DateTime? ExpiresAt, bool IsActive, AuthTokenPermissions Permissions);

}
