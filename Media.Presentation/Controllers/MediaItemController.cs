using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
using Media.Core.Dtos.Exchange;
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
        [HttpGet]
        [Route("{id}/preview")]
        [ProducesResponseType(typeof(FileStream), StatusCodes.Status200OK)]
        public async Task<FileStream> GetFileStreamPreview([FromRoute] Guid id)
        {
            var mediaFile = await this._mediaItemService.GetMediaItemFileStreamPreview(id);
            return mediaFile.FileStream;
        }

        /// <summary>
        /// Gets a download stream of the media item.
        /// </summary>
        [HttpGet]
        [Route("{id}/download")]
        public async Task<FileStreamResult> GetFileStreamDownload([FromRoute] Guid id)
        {
            var mediaFile = await this._mediaItemService.GetMediaItemFileStreamDownload(id);
            this.Response.Headers.Append("Content-Disposition", $"attachment; filename={mediaFile.FileName}");
            return this.File(mediaFile.FileStream, mediaFile.MimeType);
        }

        /// <summary>
        /// Gets metadata of the media item.
        /// </summary>
        [HttpGet]
        [Route("{id}/info")]
        public async Task<GetMediaItemInfoResponse> GetFileInfo([FromRoute] Guid id)
        {
            var mediaFile = await this._mediaItemService.GetInfo(id);
            return mediaFile;
        }

        /// <summary>
        /// Uploads a media item to the file storage.
        /// </summary>
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
        [HttpGet]
        [Route("items-by-tokens")]
        [TokenValid]
        public async Task<GetMediaItemsByTokenResponse> GetItemsByToken([FromQuery] PaginationReq pagination)
        {
            string token = this.Request.Headers.Authorization.ToString();
            return await this._mediaItemService.ByToken(token, pagination);
        }

        /// <summary>
        /// Modifies the title or description of a media item.
        /// </summary>
        [HttpPut]
        [Route("{id}/modify")]
        [TokenValid]
        public async Task<IActionResult> ModifyMediaItem([FromRoute] Guid id, [FromBody] ModifyMediaItemRequest modifyMediaItemReq)
        {
            string token = this.Request.Headers.Authorization.ToString();
            await this._mediaItemService.ModifyById(id, token, modifyMediaItemReq);
            return this.Ok();
        }

        /// <summary>
        /// Deletes the MediaItem data record and the folder/contents from the id.
        /// </summary>
        [HttpDelete]
        [Route("{id}/delete")]
        [TokenValid]
        public async Task<IActionResult> DeleteMediaItem([FromRoute] Guid id)
        {
            string token = this.Request.Headers.Authorization.ToString();
            await this._mediaItemService.DeleteById(id, token);
            return this.Ok();
        }
    }
}
