using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Media.Presentation.Controllers
{
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
        /// Uploads a media item to the file storage.
        /// </summary>
        /// <returns>Id of the created media item.</returns>
        [HttpPost]
        [Route("upload")]
        public async Task<UploadMediaItemResponse> UploadMediaItem([FromForm] UploadMediaItemRequest mediaItemReq) =>
            await this._mediaItemService.UploadMediaItem(mediaItemReq);
    }
}
