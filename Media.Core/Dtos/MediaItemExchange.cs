using Microsoft.AspNetCore.Http;

namespace Media.Core.Dtos
{
    /// <summary>
    /// Input for uploading a media item.
    /// </summary>
    /// <param name="Title">Optional title of the media file.</param>
    /// <param name="Description">Optional description of the media file.</param>
    /// <param name="FormFile">The actual media file.</param>
    public record UploadMediaItemRequest(string? Title, string? Description, IFormFile FormFile);

    /// <summary>
    /// Output for uploading a media item.
    /// </summary>
    /// <param name="Result">The Id of the created media item.</param>
    public record UploadMediaItemResponse(Guid Id);

    /// <summary>
    /// Output for file retrieval. Contains metadata about the file.
    /// </summary>
    /// <param name="FileStream">Stream of the media.</param>
    public record GetMediaItemPreviewResponse(FileStream FileStream);
    
    /// <summary>
    /// Output for file retrieval. Contains metadata about the file.
    /// </summary>
    /// <param name="FileStream">Stream of the media.</param>
    /// <param name="FileName">File name of the media.</param>
    /// <param name="MimeType">MimeType of the media.</param>
    public record GetMediaItemDownloadResponse(FileStream FileStream, string FileName, string MimeType);

    /// <summary>
    /// Output for media retrieval. Contains all files created by a token.
    /// </summary>
    /// <param name="Items">List of all items.</param>
    public record GetMediaItemsByTokenResponse(List<MinimumMediaItemDto> Items);
}

