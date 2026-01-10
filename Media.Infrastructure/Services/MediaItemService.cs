using Media.Abstractions.Interfaces;
using Media.Core;
using Media.Core.Dtos;
using Media.Core.Dtos.Exchange;
using Media.Core.Entities;
using Media.Core.Exceptions;
using Media.Core.Options;
using Media.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Media.Test.Unit")]
namespace Media.Infrastructure.Services
{
    public class MediaItemService : IMediaItemService
    {
        private readonly IAuthTokenService _tokenService;
        private readonly IFileService _fileService;
        private readonly MediaDbContext _context;
        private readonly UploadPolicyOptions _options;

        public MediaItemService(IAuthTokenService tokenService, MediaDbContext context,
            IFileService fileService, IOptions<UploadPolicyOptions> options)
        {
            this._tokenService = tokenService;
            this._fileService = fileService;
            this._context = context;
            this._options = options.Value;
        }

        /// <summary>
        /// Upload a media item and store it locally.
        /// </summary>
        /// <param name="mediaItemReq">Uploading info.</param>
        /// <param name="token">Token to proof you have writing rights.</param>
        /// <returns>Created media item object.</returns>
        public async virtual Task<UploadMediaItemResponse> UploadMediaItem(UploadMediaItemRequest mediaItemReq, string token, CancellationToken cancellationToken = default)
        {
            this.CheckFormFileValid(mediaItemReq.FormFile);

            var tokenInfo = await this._tokenService.FindTokenInfo(token, cancellationToken);
            if (!tokenInfo.Permissions.HasFlag(AuthTokenPermissions.CanCreate))
                throw new UnauthorizedException(ErrorMessages.CouldNotActionMediaMissingPermission("upload", "CanCreate"));

            var mediaItem = new MediaItem
            {
                Id = Guid.NewGuid(),
                CreatedByToken = token,
                Title = mediaItemReq.Title,
                Description = mediaItemReq.Description
            };

            using var transaction = await this._context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await this._fileService.UploadFile(mediaItem.Id, mediaItemReq.FormFile, cancellationToken);
                this._context.MediaItems.Add(mediaItem);
                
                // Save it.
                await this._context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                // Undo everything.
                this._fileService.DeleteFolder(mediaItem.Id);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

            return new(mediaItem.Id);
        }

        /// <summary>
        /// Gets the filestream. If it can't be previewed it will return notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        public async Task<GetMediaItemPreviewResponse> GetMediaItemFileStreamPreview(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._context.MediaItems.SingleOrDefaultAsync(mediaItem => mediaItem.Id == id, cancellationToken);
            if (item == null)
                throw new NotFoundException(ErrorMessages.MediaItemNotFound());

            return this._fileService.GetFileStreamPreview(id);
        }

        /// <summary>
        /// Gets the download filestream. If it can't be previewed it will return notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Download information about the file.</returns>
        public async Task<GetMediaItemDownloadResponse> GetMediaItemFileStreamDownload(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._context.MediaItems.SingleOrDefaultAsync(mediaItem => mediaItem.Id == id, cancellationToken);
            if (item == null)
                throw new NotFoundException(ErrorMessages.MediaItemNotFound());

            return this._fileService.GetFileStreamDownload(id);
        }

        /// <summary>
        /// Gets info of the MediaItem.
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Meta info of the MediaItem.</returns>
        public async Task<GetMediaItemInfoResponse> GetInfo(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._context.MediaItems.SingleOrDefaultAsync(mediaItem => mediaItem.Id == id, cancellationToken);
            if (item == null)
                throw new NotFoundException(ErrorMessages.MediaItemNotFound());

            return new GetMediaItemInfoResponse(item.CreatedByToken, item.Title, item.Description);
        }

        /// <summary>
        /// Gets all media items created by the given token.
        /// </summary>
        /// <param name="token">Token to look for.</param>
        /// <param name="pagination">Pagination object.</param>
        /// <returns>List of all media items.</returns>
        public async Task<GetMediaItemsByTokenResponse> ByToken(string token, PaginationReq pagination, CancellationToken cancellationToken = default)
        {
            var skipCount = (pagination.PageNumber - 1) * pagination.PageSize;
            
            var items = await this._context.MediaItems
                .Where(mediaItem => mediaItem.CreatedByToken == token)
                .OrderBy(mediaItem => mediaItem.CreatedOn)
                .Skip(skipCount)
                .Take(pagination.PageSize)
                .ToListAsync(cancellationToken);

            var totalItems = await this._context.MediaItems
                .Where(mediaItem => mediaItem.CreatedByToken == token)
                .CountAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling((double)totalItems / pagination.PageSize);

            return new GetMediaItemsByTokenResponse(
                Items: items.Select(mediaItem => new MinimumMediaItemDto
                {
                    Id = mediaItem.Id,
                    Title = mediaItem.Title,
                    CreatedOn = mediaItem.CreatedOn,
                    UpdatedOn = mediaItem.UpdatedOn
                }).ToList(),
                Pagination: new PaginationRes(pagination.PageNumber, pagination.PageSize, totalItems, totalPages)
            );
        }

        /// <summary>
        /// Deletes the MediaItem data record and the folder/contents from the id.
        /// </summary>
        /// <param name="id">Id of the MediaItem to delete.</param>
        /// <param name="token">Token to proof you have the correct deletion rights.</param>
        public async Task DeleteById(Guid id, string token, CancellationToken cancellationToken = default)
        {
            // Validation.
            var item = await this._context.MediaItems.SingleOrDefaultAsync(mediaItem => mediaItem.Id == id, cancellationToken);
            if (item == null)
                throw new NotFoundException(ErrorMessages.MediaItemNotFound());

            var tokenInfo = await this._tokenService.FindTokenInfo(token, cancellationToken);
            if (!tokenInfo.Permissions.HasFlag(AuthTokenPermissions.CanDelete))
                throw new UnauthorizedException(ErrorMessages.CouldNotActionMediaMissingPermission("delete", "CanDelete"));

            if (item.CreatedByToken != token)
                throw new UnauthorizedException(ErrorMessages.CouldNotActionMediaUserIsNotOwner("delete"));

            using var transaction = await this._context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                this._context.MediaItems.Remove(item);
                await this._context.SaveChangesAsync(cancellationToken);
             
                this._fileService.DeleteFolder(id);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                // Undo everything.
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// Modifies the MediaItem data record.
        /// </summary>
        /// <param name="id">Id of the MediaItem to modify.</param>
        /// <param name="token">Token to proof you have the correct deletion rights.</param>
        /// <param name="modifyMediaItemReq">new title and description</param>
        public async Task ModifyById(Guid id, string token, ModifyMediaItemRequest modifyMediaItemReq, CancellationToken cancellationToken = default)
        {
            // Validation.
            var item = await this._context.MediaItems.SingleOrDefaultAsync(mediaItem => mediaItem.Id == id, cancellationToken);
            if (item == null)
                throw new NotFoundException(ErrorMessages.MediaItemNotFound());

            var tokenInfo = await this._tokenService.FindTokenInfo(token, cancellationToken);
            if (!tokenInfo.Permissions.HasFlag(AuthTokenPermissions.CanModify))
                throw new UnauthorizedException(ErrorMessages.CouldNotActionMediaMissingPermission("modify", "CanModify"));

            if (item.CreatedByToken != token)
                throw new UnauthorizedException(ErrorMessages.CouldNotActionMediaUserIsNotOwner("modify"));

            // Update the media items. Note: they can be set to null
            item.Title = modifyMediaItemReq.Title;
            item.Description = modifyMediaItemReq.Description;

            // Update the database.
            this._context.MediaItems.Update(item);
            await this._context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Ensures the form is less then the maximum allowed file size, and 
        /// is not a blocked filetype.
        /// </summary>
        /// <param name="formFile">File to check.</param>
        internal void CheckFormFileValid(IFormFile formFile)
        {
            if (formFile.Length > this._options.MaxFileSize)
                throw new BadRequestException(ErrorMessages.FileTooLarge(this._options.MaxFileSize));

            var extension = Path.GetExtension(formFile.FileName);
            if (this._options.BlockedFileExtensions.Any(be => be.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                throw new BadRequestException(ErrorMessages.FileExtensionNotAllowed(extension));
        }
    }
}
