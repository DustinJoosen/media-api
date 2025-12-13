using Media.Core.Dtos;
using Microsoft.AspNetCore.Http;

namespace Media.Abstractions.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Gets the filestream.
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        GetMediaItemResponse GetFile(Guid id);

        /// <summary>
        /// Uploads a formfile to the file storage.
        /// </summary>
        /// <param name="id">Id to save the file as.</param>
        /// <param name="formFile">File to upload.</param>
        Task UploadFile(Guid id, IFormFile formFile);
    }
}
