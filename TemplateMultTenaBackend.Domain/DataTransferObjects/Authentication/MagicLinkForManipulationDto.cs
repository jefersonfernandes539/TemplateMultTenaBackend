namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication
{
    public record MagicLinkForManipulationDto
    {
        public Guid UserId { get; set; }
    }
}
