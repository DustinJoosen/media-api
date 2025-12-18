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
        /// Gets the filestream.
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        GetMediaItemResponse GetMediaItemFile(Guid id);
        
        /// <summary>
        /// Gets all media items created by the given token.
        /// </summary>
        /// <param name="token">Token to look for.</param>
        /// <returns>List of all media items.</returns>
        Task<GetMediaItemsByTokenResponse> ByToken(string token);
    }
}
