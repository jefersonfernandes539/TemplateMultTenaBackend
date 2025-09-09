using TemplateMultTenaBackend.Domain.DataTransferObjects.User;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Tenant
{
    public record TenantDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public UserDto? Owner { get; init; }
    }
}
