using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
using Media.Core.Entities;
using Media.Core.Exceptions;
using Media.Persistence;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Infrastructure.Services
{
    public class MediaItemService : IMediaItemService
    {
        private readonly IAuthTokenService _tokenService;
        private readonly IUploadService _uploadService;
        private readonly MediaDbContext _context;
        public MediaItemService(IAuthTokenService tokenService, MediaDbContext context, IUploadService uploadService)
        {
            this._tokenService = tokenService;
            this._uploadService = uploadService;
            this._context = context;
        }

        /// <summary>
        /// Upload a media item and store it locally.
        /// </summary>
        /// <param name="mediaItemReq">Uploading info.</param>
        /// <param name="token">Token to proof you have writing rights.</param>
        /// <returns>Created media item object.</returns>
        public async virtual Task<UploadMediaItemResponse> UploadMediaItem(UploadMediaItemRequest mediaItemReq, string token)
        {
            var tokenInfo = await this._tokenService.FindTokenInfo(new FindTokenInfoRequest(token));
            if (!tokenInfo.IsActive)
                throw new UnauthorizedException("Could not upload this media item. Provided token is deactivated.");

            if (tokenInfo.ExpiresAt < DateTime.Now)
                throw new UnauthorizedException("Could not upload this media item. Provided token is expired.");

            if (!tokenInfo.Permissions.HasFlag(AuthTokenPermissions.CanCreate))
                throw new UnauthorizedException("Could not upload this media item. Provided token does not have the CanCreate permission.");

            var mediaItem = new MediaItem
            {
                Id = Guid.NewGuid(),
                CreatedByToken = token,
                Title = mediaItemReq.Title,
                Description = mediaItemReq.Description
            };

            await this._uploadService.UploadFile(mediaItem.Id, mediaItemReq.FormFile);

            this._context.MediaItems.Add(mediaItem);
            await this._context.SaveChangesAsync();

            return new(mediaItem.Id);
        }
    }
}
