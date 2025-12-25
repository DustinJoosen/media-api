using Media.Abstractions.Interfaces;

namespace Media.Infrastructure.Services
{
    public class TestingFileService : ReleaseFileService, IFileService
    {
        /// <summary>
        /// Returns the Windows folder where the files are saved for the unit tests.
        /// </summary>
        /// <returns>Filepath of the images.</returns>
        protected override string GetFileFolder() =>
            Path.Combine(Path.GetTempPath(), "media-api-tests");
    }
}
