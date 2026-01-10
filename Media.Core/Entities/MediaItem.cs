using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Media.Core.Entities
{
    public class MediaItem : AuditableEntity
    {
        [Key, Required]
        public Guid Id { get; set; }

        [Required, ForeignKey(nameof(CreatedBy))]
        public required string CreatedByToken { get; set; }
        public AuthToken? CreatedBy { get; set; }

        [StringLength(64)]
        public string? Title { get; set; }

        [StringLength(512)]
        public string? Description { get; set; }
    }
}
