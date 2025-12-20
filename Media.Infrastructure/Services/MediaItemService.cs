using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
using Media.Core.Entities;
using Media.Core.Exceptions;
using Media.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Media.Infrastructure.Services
{
    public class MediaItemService : IMediaItemService
    {
        private readonly IAuthTokenService _tokenService;
        private readonly IFileService _fileService;
        private readonly MediaDbContext _context;
        public MediaItemService(IAuthTokenService tokenService, MediaDbContext context, IFileService fileService)
        {
            this._tokenService = tokenService;
            this._fileService = fileService;
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
            var tokenInfo = await this._tokenService.FindTokenInfo(token);
            if (!tokenInfo.Permissions.HasFlag(AuthTokenPermissions.CanCreate))
                throw new UnauthorizedException("Could not upload this media item. Provided token does not have the CanCreate permission.");

            var mediaItem = new MediaItem
            {
                Id = Guid.NewGuid(),
                CreatedByToken = token,
                Title = mediaItemReq.Title,
                Description = mediaItemReq.Description
            };

            await this._fileService.UploadFile(mediaItem.Id, mediaItemReq.FormFile);

            this._context.MediaItems.Add(mediaItem);
            await this._context.SaveChangesAsync();

            return new(mediaItem.Id);
        }

        /// <summary>
        /// Gets the filestream. If it can't be previewed it will return notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        public GetMediaItemPreviewResponse GetMediaItemFileStreamPreview(Guid id) =>
            this._fileService.GetFileStreamPreview(id);


        /// <summary>
        /// Gets the download filestream. If it can't be previewed it will return notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Download information about the file.</returns>
        public GetMediaItemDownloadResponse GetMediaItemFileStreamDownload(Guid id) =>
            this._fileService.GetFileStreamDownload(id);


        /// <summary>
        /// Gets all media items created by the given token.
        /// </summary>
        /// <param name="token">Token to look for.</param>
        /// <returns>List of all media items.</returns>
        public async Task<GetMediaItemsByTokenResponse> ByToken(string token)
        {
            var items = await this._context.MediaItems
                .Where(mediaItem => mediaItem.CreatedByToken == token)
                .ToListAsync();

            return new GetMediaItemsByTokenResponse(items.Select(mediaItem => new MinimumMediaItemDto
            {
                Id = mediaItem.Id,
                Title = mediaItem.Title,
                CreatedOn = mediaItem.CreatedOn,
                UpdatedOn = mediaItem.UpdatedOn
            }).ToList());
        }

        /// <summary>
        /// Deletes the MediaItem data record and the folder/contents from the id.
        /// </summary>
        /// <param name="id">Id of the MediaItem to delete.</param>
        /// <param name="token">Token to proof you have the correct deletion rights.</param>
        public async Task DeleteById(Guid id, string token)
        {
            // Validation.
            var item = await this._context.MediaItems.SingleOrDefaultAsync(mediaItem => mediaItem.Id == id);
            if (item == null)
                throw new NotFoundException("Media Item is not found");

            var tokenInfo = await this._tokenService.FindTokenInfo(token);
            if (!tokenInfo.Permissions.HasFlag(AuthTokenPermissions.CanDelete))
                throw new UnauthorizedException("Could not delete this media item. Provided token does not have the CanDelete permission.");

            if (item.CreatedByToken != token)
                throw new UnauthorizedException("Could not delete this media item. Provided token does not own media item.");

            // Deletion of data record.
            this._context.MediaItems.Remove(item);
            await this._context.SaveChangesAsync();

            // Deletion of folder.
            this._fileService.DeleteFolder(id);
        }

    }
}
