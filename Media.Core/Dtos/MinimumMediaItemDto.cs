using Media.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Core.Dtos
{
    public class MinimumMediaItemDto : AuditableEntity
    {
        public required Guid Id { get; set; }
        public string? Title { get; set; }
    }
}
