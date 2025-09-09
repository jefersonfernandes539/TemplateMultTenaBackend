using System.ComponentModel.DataAnnotations;
using TemplateMultTenaBackend.Domain.Attributes;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Tenant
{
    public record TenantForUpdateDto : TenantForManipulationDto
    {
        [Required(ErrorMessage = "Tenant OwnerId is a required field.")]
        [NotEmpty]
        public Guid OwnerId { get; init; }
    }
}
