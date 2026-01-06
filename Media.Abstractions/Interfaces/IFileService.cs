using Media.Core.Dtos.Exchange;
using Microsoft.AspNetCore.Http;

namespace Media.Abstractions.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Gets the filestream. If it can't be previewed it returns notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        GetMediaItemPreviewResponse GetFileStreamPreview(Guid id);

        /// <summary>
        /// Gets the downloadable filestream. If it can't be previewed it returns notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Download information about the file.</returns>
        GetMediaItemDownloadResponse GetFileStreamDownload(Guid id);

        /// <summary>
        /// Uploads a formfile to the file storage.
        /// </summary>
        /// <param name="id">Id to save the file as.</param>
        /// <param name="formFile">File to upload.</param>
        Task UploadFile(Guid id, IFormFile formFile, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Recursively deletes the folder of the given item.
        /// </summary>
        /// <param name="id">Id of the folder to delete</param>
        void DeleteFolder(Guid id);
    }
}
