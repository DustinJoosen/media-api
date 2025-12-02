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
    /// Input for finding token expiration.
    /// </summary>
    /// <param name="Token">Token to find.</param>
    public record FindTokenExpirationRequest(string Token);

    /// <summary>
    /// Output for finding token expiration.
    /// </summary>
    /// <param name="Token">Expiration date.</param>
    public record FindTokenExpirationResponse(DateTime? ExpiresAt);

}
