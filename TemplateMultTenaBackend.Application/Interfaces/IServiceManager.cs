using Microsoft.AspNetCore.Identity;
using TemplateMultTenaBackend.Domain.Entities;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IAuthorizationService AuthorizationService { get; }
        IJwtService JwtService { get; }
        IUserRefreshTokenService UserRefreshTokenService { get; }
        RoleManager<Role> RoleManager { get; }
        ITenantService TenantService { get; }
        ITenantUserService TenantUserService { get; }
        IMagicLinkService MagicLinkService { get; }
        IUserService UserService { get; }
    }
}
