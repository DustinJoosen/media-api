using Media.Abstractions.Interfaces;
using Media.Core.Dtos.Exchange;
using Media.Core.Entities;
using Media.Core.Exceptions;
using Media.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Media.Infrastructure.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly MediaDbContext _context;
        public AuthTokenService(MediaDbContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Create an authorization token.
        /// </summary>
        /// <param name="tokenReq">Token creation information.</param>
        /// <returns>Created token.</returns>
        public async virtual Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq, CancellationToken cancellationToken = default)
        {
            if (await this.IsNameUsed(tokenReq.Name))
                throw new AlreadyUsedException($"Token name '{tokenReq.Name}' is already being used");

            var token = this.GenerateSecureToken();

            this._context.AuthTokens.Add(new AuthToken
            {
                Token = token,
                Name = tokenReq.Name,
                ExpiresAt = tokenReq.ExpiresAt,
                Permissions = (AuthTokenPermissions)15,
                IsActive = true
            });

            await this._context.SaveChangesAsync(cancellationToken);
            return new(token);
        }

        /// <summary>
        /// Finds the info of an authorization token.
        /// </summary>
        /// <param name="token">Token to find information of.</param>
        /// <returns>Info of the token.</returns>
        public async virtual Task<FindTokenInfoResponse> FindTokenInfo(string token, CancellationToken cancellationToken = default)
        {
            var authToken = await this._context.AuthTokens.SingleOrDefaultAsync(at => at.Token == token, cancellationToken);
            if (authToken == null)
                throw new NotFoundException($"Token '{token}' does not exist");

            return new(authToken.Name, authToken.ExpiresAt, authToken.IsActive, authToken.Permissions);
        }

        /// <summary>
        /// Deactivates an authorization token.
        /// </summary>
        /// <param name="token">Token to deactivate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public async virtual Task DeactivateToken(string token, CancellationToken cancellationToken = default)
        {
            var authToken = await this._context.AuthTokens.SingleOrDefaultAsync(at => at.Token == token, cancellationToken);
            if (authToken == null)
                throw new NotFoundException($"Token '{token}' does not exist");

            try
            {
                authToken.IsActive = false;
                this._context.AuthTokens.Update(authToken);
                await this._context.SaveChangesAsync(cancellationToken);
            } 
            catch
            {
                throw new DatabaseOperationException($"Could not deactivate token");
            }
        }

        /// <summary>
        /// Gets the permissions an auth token has.
        /// </summary>
        /// <param name="token">Token to find permissions from.</param>
        /// <returns>Permissions object of the given token.</returns>
        public async Task<AuthTokenPermissions> GetRoles(string token, CancellationToken cancellationToken = default)
        {
            var authToken = await this._context.AuthTokens.SingleOrDefaultAsync(at => at.Token == token, cancellationToken);
            if (authToken == null)
                throw new NotFoundException($"Token '{token}' does not exist");

            return authToken.Permissions;
        }

        /// <summary>
        /// Changes permissions of an authorization token. Note that this action requires a token with the CanManagePermissions permission.
        /// </summary>
        /// <param name="changeTokenPermissionReq">The token to change, and the new permissions.</param>
        /// <param name="token">Token to proof the user is allowed to change permissions. NOT THE TOKEN THAT WILL BE CHANGED.</param>
        public async Task ChangePermissions(ChangeTokenPermissionRequest changeTokenPermissionReq, string token, CancellationToken cancellationToken = default)
        {
            // Check that you're allowed to change the permissions.
            var roles = await this.GetRoles(token);
            if (!roles.HasFlag(AuthTokenPermissions.CanManagePermissions))
                throw new UnauthorizedException("Provided token does not have the needed permissions.");

            // Find and check the existence of the token.
            var authToken = await this._context.AuthTokens.SingleOrDefaultAsync(at => at.Token == changeTokenPermissionReq.Token, cancellationToken);
            if (authToken == null)
                throw new NotFoundException($"Token '{changeTokenPermissionReq.Token}' does not exist");

            // Update the new permission.
            try
            {
                authToken.Permissions = changeTokenPermissionReq.Permission;
                this._context.AuthTokens.Update(authToken);
                await this._context.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                throw new DatabaseOperationException($"Could not update token permissions");
            }
        }

        /// <summary>
        /// Check if a token already is named this.
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <returns>Whether the given name is already in use.</returns>
        private async Task<bool> IsNameUsed(string name, CancellationToken cancellationToken = default) =>
            await this._context.AuthTokens.AnyAsync(at => at.Name == name, cancellationToken);

        /// <summary>
        /// Randomly generates a secure token of 64 characters long.
        /// </summary>
        /// <returns>Generated token.</returns>
        private string GenerateSecureToken()
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] data = new byte[64];
            rng.GetBytes(data);

            return Convert.ToBase64String(data);
        }
    }
}
