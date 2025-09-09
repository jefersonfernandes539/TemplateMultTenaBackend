using System.ComponentModel.DataAnnotations;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.User
{
    public record UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string EmailOrPhoneNumber { get; init; } = null!;

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Password { get; init; } = null!;

        public Guid TenantId { get; init; }
    }
}
