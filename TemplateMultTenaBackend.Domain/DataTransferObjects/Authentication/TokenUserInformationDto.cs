using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TemplateMultTenaBackend.Domain.Attributes;

namespace TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication
{
    public record TokenUserInformationDto
    {
        public TokenUserInformationDto() { }
        public TokenUserInformationDto(ClaimsPrincipal principal)
        {
            UserId = Guid.Parse(principal.FindFirst(ClaimTypes.Name)!.Value!);
            TenantId = Guid.Empty;
            RoleId = Guid.Empty;

            if (principal.FindFirst("Tenant.Id") != null)
                TenantId = Guid.Parse(principal.FindFirst("Tenant.Id")?.Value!);

            if (principal.FindFirst("TenantUser.RoleId") != null)
                RoleId = Guid.Parse(principal.FindFirst("TenantUser.RoleId")?.Value!);
        }

        [NotEmpty, Required]
        public Guid UserId { get; init; }

        public Guid TenantId { get; init; }
        public Guid RoleId { get; init; }
    }
}
