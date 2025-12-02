using Media.Core.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Abstractions.Interfaces
{
    public interface IMediaItemService
    {
        /// <summary>
        /// Upload a media item and store it locally.
        /// </summary>
        /// <param name="mediaItemReq">Uploading info.</param>
        /// <returns>Created media item object.</returns>
        Task<UploadMediaItemResponse> UploadMediaItem(UploadMediaItemRequest mediaItemReq);
    }
}
