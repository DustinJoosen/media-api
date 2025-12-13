using Media.Abstractions.Interfaces;
using Media.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
