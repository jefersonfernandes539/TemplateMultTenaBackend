namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication
{
    public record PasswordResetDto
    {
        public Guid UserId { get; init; }
        public string Token { get; init; } = string.Empty;
        public string NewPassword { get; init; } = string.Empty;
    }
}
