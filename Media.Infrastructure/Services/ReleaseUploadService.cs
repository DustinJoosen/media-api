using Media.Abstractions.Interfaces;
using Media.Core.Exceptions;
using Microsoft.AspNetCore.Http;


namespace Media.Infrastructure.Interfaces
{
    public class ReleaseUploadService : IUploadService
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
            catch (UnauthorizedAccessException ex)
            {
                throw new BadRequestException($"Access to the file path is denied");
            }
        }

        /// <summary>
        /// Returns the Linux folder where the files are saved.
        /// </summary>
        /// <returns>Filepath of the images.</returns>
        protected virtual string GetFileFolder() =>
            "/home/syter/Pictures/media";
    }
}
