using Media.Abstractions.Interfaces;
using Media.Core.Dtos;
using Media.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Media.Infrastructure.Services
{
    public class ReleaseFileService : IFileService
    {
        /// <summary>
        /// Uploads a formfile to the file storage.
        /// </summary>
        /// <param name="formFile">File to upload.</param>
        public async Task UploadFile(Guid id, IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
                throw new BadRequestException("Uploaded file is null or has a length of 0");

            var uploadsFolder = this.GetFileFolder();

            var folder = Path.Combine(uploadsFolder, id.ToString());
            var filePath = Path.Combine(folder, Path.GetFileName(formFile.FileName));

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            try
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await formFile.CopyToAsync(stream);
            }
            catch
            {
                throw new BadRequestException($"Access to the file path is denied");
            }
        }

        /// <summary>
        /// Gets the filestream.
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        public GetMediaItemResponse GetFile(Guid id)
        {
            var rootFolder = this.GetFileFolder();

            try
            {
                var folder = Path.Combine(rootFolder, id.ToString());
                var folderFiles = Directory.GetFiles(folder);
                var filePath = folderFiles.First();

                return new(File.OpenRead(filePath), this.GetMimeType(filePath));
            }
            catch
            {
                var filePath = Path.Combine(rootFolder, "notfound.png");
                return new(File.OpenRead(filePath), "image/png");
            }
        }


        /// <summary>
        /// Returns the Linux folder where the files are saved.
        /// </summary>
        /// <returns>Filepath of the images.</returns>
        protected virtual string GetFileFolder() =>
            "/home/syter/Pictures/media";

        /// <summary>
        /// Gets the correct mimetype of the file extension.
        /// </summary>
        /// <param name="filePath">Filepath to check the extension of.</param>
        /// <returns>Correct mimetype.</returns>
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".json" => "application/json",
                _ => "application/octet-stream",
            };
        }
    }
}
