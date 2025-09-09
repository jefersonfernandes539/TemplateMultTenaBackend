using Microsoft.EntityFrameworkCore;
using TemplateMultTenaBackend.Application.Attributes;

namespace TemplateMultTenaBackend.Domain.Entities
{
    [Index(nameof(UserId), nameof(TenantId), IsUnique = true)]
    public class TenantUser : BaseModel
    {
        [NotEmpty]
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;

        [NotEmpty]
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        [NotEmpty]
        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;

        [NotEmpty]
        public Guid CreatedById { get; set; }

        public User CreatedBy { get; set; } = null!;

        [NotEmpty]
        public Guid UpdatedById { get; set; }

        public User UpdatedBy { get; set; } = null!;
    }
}
