using TemplateMultTenaBackend.Domain.DataTransferObjects.Tenant;
using TemplateMultTenaBackend.Domain.DataTransferObjects.User;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.TenantUser
{
    public record TenantUserDto
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; } = Guid.Empty;
        public UserDto? User { get; init; }

        public Guid TenantId { get; init; } = Guid.Empty;
        public TenantDto? Tenant { get; init; }

        public Guid RoleId { get; init; } = Guid.Empty;
        public DateTime CreatedAt { get; init; }
        public UserDto? CreatedBy { get; init; }
        public Guid CreatedById { get; init; }
        public DateTime UpdatedAt { get; init; }
        public Guid UpdatedById { get; init; }
        public UserDto? UpdatedBy { get; init; }
    }
}
