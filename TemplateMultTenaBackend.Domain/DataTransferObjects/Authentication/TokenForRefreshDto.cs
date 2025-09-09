namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication
{
    public record TokenForRefreshDto : TokenDto
    {
        public Guid TenantId { get; init; } = Guid.Empty;
    }
}
