using Media.Abstractions.Interfaces;
using Media.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace Media.Test.Core.Helpers
{
    public class TestingFileService : ReleaseFileService, IFileService
    {
        public bool ShouldThrowOnUpload { get; set; }

        public override Task UploadFile(Guid id, IFormFile formFile, CancellationToken cancellationToken = default)
        {
            if (this.ShouldThrowOnUpload)
                throw new Exception("Could not upload the file on the testing file service.");

            return base.UploadFile(id, formFile, cancellationToken);
        }

        /// <summary>
        /// Returns the Windows folder where the files are saved for the unit tests.
        /// </summary>
        /// <returns>Filepath of the images.</returns>
        protected override string GetFileFolder() =>
            Path.Combine(Path.GetTempPath(), "media-api-tests");
    }
}
