using System.ComponentModel.DataAnnotations;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Tenant
{
    public abstract record TenantForManipulationDto
    {
        [Required(ErrorMessage = "Tenant name is a required field.")]
        [MaxLength(32, ErrorMessage = "Maximum length for the Name is 32 characters.")]
        public string Name { get; init; } = null!;
    }
}
