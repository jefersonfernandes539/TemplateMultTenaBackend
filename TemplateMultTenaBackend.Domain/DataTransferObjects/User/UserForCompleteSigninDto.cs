using System.ComponentModel.DataAnnotations;
using TemplateMultTenaBackend.Domain.Attributes;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.User
{
    public record UserForCompleteSigninDto
    {
        [NotEmpty, Required(ErrorMessage = "First name is required")]
        public string FirstName { get; init; } = null!;

        [NotEmpty, Required(ErrorMessage = "Last name is required")]
        public string LastName { get; init; } = null!;

        [NotEmpty, Required(ErrorMessage = "Password is required")]
        public string NewPassword { get; init; } = null!;
    }
}
