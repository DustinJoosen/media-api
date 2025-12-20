using Media.Core.Dtos;

namespace Media.Abstractions.Interfaces
{
    public interface IMediaItemService
    {
        /// <summary>
        /// Upload a media item and store it locally.
        /// </summary>
        /// <param name="mediaItemReq">Uploading info.</param>
        /// <param name="token">Token to proof you have writing rights.</param>
        /// <returns>Created media item object.</returns>
        Task<UploadMediaItemResponse> UploadMediaItem(UploadMediaItemRequest mediaItemReq, string token);

        /// <summary>
        /// Gets the filestream. If it can't be previewed it will return notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        Task<GetMediaItemPreviewResponse> GetMediaItemFileStreamPreview(Guid id);
        
        /// <summary>
        /// Gets the download filestream. If it can't be previewed it will return notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Download information about the file.</returns>
        Task<GetMediaItemDownloadResponse> GetMediaItemFileStreamDownload(Guid id);
        
        /// <summary>
        /// Gets all media items created by the given token.
        /// </summary>
        /// <param name="token">Token to look for.</param>
        /// <returns>List of all media items.</returns>
        Task<GetMediaItemsByTokenResponse> ByToken(string token);

        /// <summary>
        /// Deletes the MediaItem data record and the folder/contents from the id.
        /// </summary>
        /// <param name="id">Id of the MediaItem to delete.</param>
        /// <param name="token">Token to proof you have the correct deletion rights.</param>
        Task DeleteById(Guid id, string token);
    }
}
