namespace TemplateMultTenaBackend.Domain.DataTransferObjects
{
    public record RequestIdentificationDto
    {
        public string? IpAddress { get; init; } = string.Empty;
        public string? UserAgent { get; init; } = string.Empty;
    }
}
