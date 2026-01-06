using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Core.Options
{
    public class UploadPolicyOptions
    {
        public long MaxFileSize { get; set; }
        public List<string> BlockedFileExtensions { get; set; }

        public UploadPolicyOptions() =>
            this.BlockedFileExtensions = new();

        public UploadPolicyOptions(long maxFileSize, List<string> blockedFileExtensions)
        {
            this.MaxFileSize = maxFileSize;
            this.BlockedFileExtensions = blockedFileExtensions;
        }
    }
}
