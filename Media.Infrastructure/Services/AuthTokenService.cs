using Media.Abstractions.Interfaces;
using Media.Core.Entities;
using Media.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Guid> CreateToken()
        {
            Guid token = Guid.NewGuid();
            this._context.AuthTokens.Add(new AuthToken
            {
                Token = token,
                Permissions = AuthTokenPermissions.CanCreate
            });

            await this._context.SaveChangesAsync();
            return token;
        }
    }
}
