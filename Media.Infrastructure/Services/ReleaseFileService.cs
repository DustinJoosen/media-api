using Media.Abstractions.Interfaces;
using Media.Core.Dtos.Exchange;
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
        public virtual async Task UploadFile(Guid id, IFormFile formFile, CancellationToken cancellationToken = default)
        {
            if (formFile == null || formFile.Length == 0)
                throw new BadRequestException("Uploaded file is null or has a length of 0");

            var guidFolder = this.GetFullGuidFolder(id);
            var filePath = Path.Combine(guidFolder, Path.GetFileName(formFile.FileName));

            try
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await formFile.CopyToAsync(stream, cancellationToken);
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
            var guidFolder = this.GetFullGuidFolder(id);

            if (!Directory.Exists(guidFolder))
                throw new NotFoundException("File could not be deleted, it does not exist");

            Directory.Delete(guidFolder, recursive: true);
        }

        /// <summary>
        /// Gets the first (hopefully only) file in the folder.
        /// </summary>
        /// <param name="id">Id of the folder to find</param>
        /// <returns>Full filepath of the first file.</returns>
        protected virtual string GetFirstFileFromId(Guid id)
        {
            var guidFolder = this.GetFullGuidFolder(id);

            try
            {
                var folderFiles = Directory.GetFiles(guidFolder);
                return folderFiles.First();
            }
            catch
            {
                var rootFolder = this.GetFileFolder();
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
        /// Get the full folder path of a media item based on guid with dashes as seperators.
        /// <code>Example
        /// 1f7179f9-d979-497b-902a-177eafbd22fc
        /// rootFolder/1f7179f9/d979/497b/902a/177eafbd22fc.
        /// </code>        
        /// This method also creates the directory.
        /// </summary>
        /// <param name="guid">Guid to parse the folder from.</param>
        /// <returns>The folder path.</returns>
        private string GetFullGuidFolder(Guid guid)
        {
            var rootFolder = this.GetFileFolder();
            string nestedPath = Path.Combine(rootFolder, 
                guid.ToString().Replace("-", Path.DirectorySeparatorChar.ToString()));

            Directory.CreateDirectory(nestedPath);
            return nestedPath;
        }

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
