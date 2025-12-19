using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
using Media.Presentation.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Media.Presentation.Controllers
{
    /// <summary>
    /// Endpoints about the media items themselves.
    /// </summary>
    [Route("media")]
    [ApiController]
    public class MediaItemController : ControllerBase
    {
        private readonly IMediaItemService _mediaItemService;
        public MediaItemController(IMediaItemService mediaItemService)
        {
            this._mediaItemService = mediaItemService;
        }

        /// <summary>
        /// Gets a preview filestream of the media item.
        /// </summary>
        /// <param name="id">Id of the media item.</param>
        /// <returns>Open filestream.</returns>
        [HttpGet]
        [Route("{id}/preview")]
        [ProducesResponseType(typeof(FileStream), StatusCodes.Status200OK)]
        public FileStream GetFileStreamPreview([FromRoute] Guid id)
        {
            var mediaFile = this._mediaItemService.GetMediaItemFileStreamPreview(id);
            return mediaFile.FileStream;
        }

        /// <summary>
        /// Gets a download stream of the media item.
        /// </summary>
        /// <param name="id">Id of the media item.</param>
        /// <returns>Download filestream.</returns>
        [HttpGet]
        [Route("{id}/download")]
        public FileStreamResult GetFileStreamDownload([FromRoute] Guid id)
        {
            var mediaFile = this._mediaItemService.GetMediaItemFileStreamDownload(id);
            this.Response.Headers.Append("Content-Disposition", $"attachment; filename={mediaFile.FileName}");
            return this.File(mediaFile.FileStream, mediaFile.MimeType);
        }

        /// <summary>
        /// Uploads a media item to the file storage.
        /// </summary>
        /// <returns>Id of the created media item.</returns>
        [HttpPost]
        [Route("upload")]
        [TokenValid]
        public async Task<UploadMediaItemResponse> UploadMediaItem([FromForm] UploadMediaItemRequest mediaItemReq)
        {
            string token = this.Request.Headers.Authorization.ToString();
            return await this._mediaItemService.UploadMediaItem(mediaItemReq, token);
        }

        /// <summary>
        /// Gets all media items created by the given token.
        /// </summary>
        /// <returns>List of all media items.</returns>
        [HttpGet]
        [Route("items-by-tokens")]
        [TokenValid]
        public async Task<GetMediaItemsByTokenResponse> GetItemsByToken()
        {
            string token = this.Request.Headers.Authorization.ToString();
            return await this._mediaItemService.ByToken(token);
        }

    }
}
