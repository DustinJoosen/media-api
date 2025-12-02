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
        public Guid Token { get; set; }

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
