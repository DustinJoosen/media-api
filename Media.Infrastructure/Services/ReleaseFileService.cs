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
        /// Gets the filestream. If it can't be previewed it returns notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        public GetMediaItemPreviewResponse GetFileStreamPreview(Guid id)
        {
            var rootFolder = this.GetFileFolder();
            var filePath = this.GetFirstFileFromId(id);

            // Prevent path traversal security attacks.
            if (!filePath.StartsWith(rootFolder, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Access to the specified file is not allowed.");

            if (!this.CanPreview(filePath))
                filePath = Path.Combine(rootFolder, "notfound.png");

            return new(File.OpenRead(filePath));
        }

        /// <summary>
        /// Gets the filestream. If it can't be previewed it returns notfound.png
        /// </summary>
        /// <param name="id">Id of the specified media item.</param>
        /// <returns>Metadata of the file: filestream, name, and mimetype.</returns>
        public GetMediaItemDownloadResponse GetFileStreamDownload(Guid id)
        {
            var filePath = this.GetFirstFileFromId(id);
            var fileName = Path.GetFileName(filePath);
            var mimeType = this.GetMimeType(filePath);

            return new GetMediaItemDownloadResponse(File.OpenRead(filePath), fileName, mimeType);
        }

        /// <summary>
        /// Recursively deletes the folder of the given item.
        /// </summary>
        /// <param name="id">Id of the folder to delete</param>
        public void DeleteFolder(Guid id)
        {
            var folderPath = Path.Combine(this.GetFileFolder(), id.ToString());

            if (!Directory.Exists(folderPath))
                throw new BadRequestException("File could not be deleted, it does not exist");

            Directory.Delete(folderPath, recursive: true);
        }

        /// <summary>
        /// Gets the first (hopefully only) file in the folder.
        /// </summary>
        /// <param name="id">Id of the folder to delete</param>
        /// <returns>Full filepath of the first file.</returns>
        protected virtual string GetFirstFileFromId(Guid id)
        {
            var rootFolder = this.GetFileFolder();

            try
            {
                var folder = Path.Combine(rootFolder, id.ToString());
                var folderFiles = Directory.GetFiles(folder);
                return folderFiles.First();
            }
            catch
            {
                return Path.Combine(rootFolder, "notfound.png");
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

        /// <summary>
        /// Checks if a file can be previewed.
        /// </summary>
        /// <param name="filePath">Filepath to check the preview ability of.</param>
        /// <returns>Whether a file can be previewed.</returns>
        private bool CanPreview(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            var previewAllowedOptions = new List<string>([".jpg", ".jpeg", ".png"]);
            return previewAllowedOptions.Contains(extension);
        }
    }
}
