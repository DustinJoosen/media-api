using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
using Media.Core.Entities;
using Media.Core.Exceptions;
using Media.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="tokenReq">token creation information.</param>
        /// <returns>Created token.</returns>
        public async virtual Task<CreateTokenResponse> CreateToken(CreateTokenRequest tokenReq)
        {
            if (await this.IsNameUsed(tokenReq.Name))
                throw new AlreadyUsedException($"Token name '{tokenReq.Name}' is already being used");

            var token = this.GenerateSecureToken();

            this._context.AuthTokens.Add(new AuthToken
            {
                Token = token,
                Name = tokenReq.Name,
                ExpiresAt = tokenReq.ExpiresAt,
                Permissions = AuthTokenPermissions.CanCreate,
                IsActive = true
            });

            await this._context.SaveChangesAsync();
            return new(token);
        }

        /// <summary>
        /// Finds the info of an authorization token.
        /// </summary>
        /// <param name="findTokenReq">token finding information.</param>
        /// <returns>info of the token.</returns>
        public async virtual Task<FindTokenInfoResponse> FindTokenInfo(FindTokenInfoRequest findTokenReq)
        {
            var authToken = await this._context.AuthTokens.SingleOrDefaultAsync(at => at.Token == findTokenReq.Token);
            if (authToken == null)
                throw new NotFoundException($"Token '{findTokenReq.Token}' does not exist");

            return new(authToken.ExpiresAt, authToken.IsActive);
        }

        /// <summary>
        /// Deactivates an authorization token.
        /// </summary>
        /// <param name="token">token to deactivate.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public async virtual Task DeactivateToken(string token)
        {
            var authToken = await this._context.AuthTokens.SingleOrDefaultAsync(at => at.Token == token);
            if (authToken == null)
                throw new NotFoundException($"Token '{token}' does not exist");

            try
            {
                authToken.IsActive = false;
                this._context.AuthTokens.Update(authToken);
                await this._context.SaveChangesAsync();
            } 
            catch
            {
                throw new DatabaseOperationException($"Could not deactivate token");
            }
        }

        /// <summary>
        /// Check if a token already is named this.
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <returns>Whether the given name is already in use.</returns>
        private async Task<bool> IsNameUsed(string name) =>
            await this._context.AuthTokens.AnyAsync(at => at.Name == name);


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
