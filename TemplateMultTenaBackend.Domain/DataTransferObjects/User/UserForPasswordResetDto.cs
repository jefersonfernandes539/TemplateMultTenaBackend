using System.ComponentModel.DataAnnotations;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.User
{
    public record UserForPasswordResetDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;
    }
}
