using Media.Abstractions.Interfaces;

namespace Media.Infrastructure.Services
{
    public class DebugFileService : ReleaseFileService, IFileService
    {
        /// <summary>
        /// Returns the Windows folder where the files are saved.
        /// </summary>
        /// <returns>Filepath of the images.</returns>
        protected override string GetFileFolder() =>
            "C:\\Users\\Dustin\\Pictures\\media-api";
    }
}
