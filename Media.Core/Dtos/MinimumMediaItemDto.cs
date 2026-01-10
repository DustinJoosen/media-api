using Media.Core.Entities;

namespace Media.Core.Dtos
{
    public class MinimumMediaItemDto : AuditableEntity
    {
        public required Guid Id { get; set; }
        public string? Title { get; set; }
    }
}
