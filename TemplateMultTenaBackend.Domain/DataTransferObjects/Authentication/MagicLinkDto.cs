namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication
{
    public record MagicLinkDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
