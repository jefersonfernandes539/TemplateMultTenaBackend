using System.ComponentModel.DataAnnotations;
using TemplateMultTenaBackend.Domain.Attributes;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.User
{
    public record UserForUpdateDto
    {
        [NotEmpty, Required(ErrorMessage = "First name is required")]
        public string FirstName { get; init; } = null!;

        [NotEmpty, Required(ErrorMessage = "Last name is required")]
        public string LastName { get; init; } = null!;

        [NotEmpty, Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; init; } = null!;

        [NotEmpty, Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Phone number is invalid")]
        public string PhoneNumber { get; init; } = null!;
    }
}
