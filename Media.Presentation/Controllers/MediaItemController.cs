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
        /// Gets a filestream of the media item.
        /// </summary>
        /// <param name="id">Id of the media item.</param>
        /// <returns>Open filestream.</returns>
        [HttpGet]
        [Route("{id}/file")]
        [ProducesResponseType(typeof(FileStream), StatusCodes.Status200OK)]
        public FileStreamResult GetFileStream([FromRoute] Guid id)
        {
            var mediaFile = this._mediaItemService.GetMediaItemFile(id);
            return this.File(mediaFile.FileStream, mediaFile.MimeType);
        }

        /// <summary>
        /// Uploads a media item to the file storage.
        /// </summary>
        /// <returns>Id of the created media item.</returns>
        [HttpPost]
        [Route("upload")]
        [TokenRequired]
        public async Task<UploadMediaItemResponse> UploadMediaItem([FromForm] UploadMediaItemRequest mediaItemReq)
        {
            string token = this.Request.Headers.Authorization.ToString();
            return await this._mediaItemService.UploadMediaItem(mediaItemReq, token);
        }
    }
}
