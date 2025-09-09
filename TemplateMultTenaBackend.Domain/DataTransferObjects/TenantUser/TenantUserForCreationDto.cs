namespace TemplateMultTenaBackend.Domain.DataTransferObjects.TenantUser
{
    public record TenantUserForCreationDto : TenantUserForManipulationDto
    {
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }
    }
}
