using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TemplateMultTenaBackend.Domain.Attributes;

namespace TemplateMultTenaBackend.Domain.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)] // TODO: Improve error message on UI --> current error 500, ideal 422
    public class User : IdentityUser<Guid>
    {
        [NotEmpty]
        public override string UserName { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [NotEmpty]
        public override string Email { get; set; } = null!;

        [Required]
        [NotEmpty]
        public override string PhoneNumber { get; set; } = null!;

        public IEnumerable<Tenant> Tenants { get; set; } = new List<Tenant>();
        public IEnumerable<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
        public IEnumerable<MagicLink> MagicLinks { get; } = new List<MagicLink>();

        public string FullName { get => $"{FirstName} {LastName}"; }
    }
}
