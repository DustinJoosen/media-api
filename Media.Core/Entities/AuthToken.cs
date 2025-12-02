using Media.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Core.Entities
{
    public class AuthToken : AuditableEntity
    {
        [Key, Required]
        public string Token { get; set; }

        [Required, StringLength(64)]
        public string Name { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [Required]
        public AuthTokenPermissions Permissions { get; set; }
    }

    public enum AuthTokenPermissions
    {
        CanRead,
        CanCreate,
        CanDelete
    }
}
