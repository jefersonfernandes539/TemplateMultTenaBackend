using System.ComponentModel.DataAnnotations;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication
{
    public record TokenDto
    {
        [Required(ErrorMessage = "AccessToken is a required field.")]
        public string AccessToken { get; init; } = null!;

        [Required(ErrorMessage = "RefreshToken is a required field.")]
        public string RefreshToken { get; init; } = null!;
    }
}
