using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Abstractions.Interfaces
{
    public interface IUploadService
    {
        /// <summary>
        /// Uploads a formfile to the file storage.
        /// </summary>
        /// <param name="id">Id to save the file as.</param>
        /// <param name="formFile">File to upload.</param>
        Task UploadFile(Guid id, IFormFile formFile);
    }
}
