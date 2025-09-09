namespace TemplateMultTenaBackend.Domain.DataTransferObjects.User
{
    public record UserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }
}
