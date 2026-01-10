using Media.Abstractions.Interfaces;
using Media.Core;
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
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>Created token.</returns>
        public async virtual Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq, 
            CancellationToken cancellationToken = default)
        {
            if (await this.IsNameUsed(tokenReq.Name, cancellationToken))
                throw new AlreadyUsedException(ErrorMessages.TokenNameAlreadyUsed(tokenReq.Name));

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
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>Info of the token.</returns>
        public async virtual Task<FindTokenInfoResponse> FindTokenInfo(string token, 
            CancellationToken cancellationToken = default)
        {
            var authToken = await this._context.AuthTokens.FindAsync(token, cancellationToken);
            if (authToken == null)
                throw new NotFoundException(ErrorMessages.TokenDoesNotExist(token));

            return new(
                Name: authToken.Name, 
                ExpiresAt: authToken.ExpiresAt, 
                IsActive: authToken.IsActive, 
                Permissions: authToken.Permissions);
        }

        /// <summary>
        /// Deactivates an authorization token.
        /// </summary>
        /// <param name="token">Token to deactivate.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        public async virtual Task DeactivateToken(string token, 
            CancellationToken cancellationToken = default)
        {
            var authToken = await this._context.AuthTokens.FindAsync(token, cancellationToken);
            if (authToken == null)
                throw new NotFoundException(ErrorMessages.TokenDoesNotExist(token));

            try
            {
                authToken.IsActive = false;
                this._context.AuthTokens.Update(authToken);
                await this._context.SaveChangesAsync(cancellationToken);
            } 
            catch
            {
                throw new DatabaseOperationException(ErrorMessages.CannotDeactivateToken(token));
            }
        }

        /// <summary>
        /// Gets the permissions an auth token has.
        /// </summary>
        /// <param name="token">Token to find permissions from.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>Permissions object of the given token.</returns>
        public async Task<AuthTokenPermissions> GetRoles(string token, 
            CancellationToken cancellationToken = default)
        {
            var authToken = await this._context.AuthTokens.FindAsync(token, cancellationToken);
            if (authToken == null)
                throw new NotFoundException(ErrorMessages.TokenDoesNotExist(token));

            return authToken.Permissions;
        }

        /// <summary>
        /// Changes permissions of an authorization token. Note that this action requires 
        /// a token with the CanManagePermissions permission.
        /// </summary>
        /// <param name="changeReq">The token to change, and the new permissions.</param>
        /// <param name="token">
        /// Token used to authorize the permission change (not the token being modified).
        /// </param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        public async Task ChangePermissions(ChangeTokenPermissionRequest changeTokenPermissionReq,
            string token, CancellationToken cancellationToken = default)
        {
            // Check that you're allowed to change the permissions.
            var roles = await this.GetRoles(token);
            if (!roles.HasFlag(AuthTokenPermissions.CanManagePermissions))
                throw new UnauthorizedException(ErrorMessages.TokenDoesNotHavePermissions());

            // Find and check the existence of the token.
            var authToken = await this._context.AuthTokens.FindAsync(changeTokenPermissionReq.Token,
				cancellationToken);
			if (authToken == null)
                throw new NotFoundException(ErrorMessages.TokenDoesNotExist(token));

            // Update the new permission.
            try
            {
                authToken.Permissions = changeTokenPermissionReq.Permission;
                this._context.AuthTokens.Update(authToken);
                await this._context.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                throw new DatabaseOperationException(ErrorMessages.CannotUpdateTokenPermissions());
            }
        }

        /// <summary>
        /// Check if a token already is named this.
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>True if the given name is already in use.</returns>
        private async Task<bool> IsNameUsed(string name, 
            CancellationToken cancellationToken = default) =>
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
