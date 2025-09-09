namespace TemplateMultTenaBackend.Domain.DataTransferObjects.TenantUser
{
    public abstract record TenantUserForManipulationDto
    {
        public Guid RoleId { get; set; }
    }
}
