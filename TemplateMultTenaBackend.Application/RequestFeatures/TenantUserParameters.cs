namespace TemplateMultTenaBackend.Application.RequestFeatures
{
    public class TenantUserParameters : RequestParameters
    {
        public TenantUserParameters() => OrderBy = "CreatedAt desc";

        public Guid RoleId { get; set; } = Guid.Empty;
    }
}
