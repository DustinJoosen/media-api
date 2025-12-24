using System.ComponentModel.DataAnnotations;

namespace Media.Core.Entities
{
    public class AuthToken : AuditableEntity
    {
        [Key, Required]
        public required string Token { get; set; }

        [Required, StringLength(64)]
        public required string Name { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [Required]
        public AuthTokenPermissions Permissions { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }

    [Flags]
    public enum AuthTokenPermissions
    {
        CanRead = 1,
        CanCreate = 2,
        CanDelete = 4,
        CanModify = 8,
        CanManagePermissions = 16
    }
}
