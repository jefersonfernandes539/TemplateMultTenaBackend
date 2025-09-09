using System.ComponentModel.DataAnnotations;
using TemplateMultTenaBackend.Domain.Attributes;

namespace TemplateMultTenaBackend.Domain.Entities
{
    public class Tenant : BaseModel
    {
        [Required(ErrorMessage = "Nome é um campo obrigatório.")]
        [MaxLength(32, ErrorMessage = "O nome deve der no máximo 32 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Proprietário é um campo obrigatório.")]
        [NotEmpty]
        public Guid OwnerId { get; set; }

        public User Owner { get; set; } = null!;

        public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    }
}
