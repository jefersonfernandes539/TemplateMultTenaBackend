using Microsoft.EntityFrameworkCore.Storage;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface IRepositoryManager
    {
        ITenantRepository Tenant { get; }
        ITenantUserRepository TenantUser { get; }
        IUserRefreshTokenRepository UserRefreshToken { get; }
        IMagicLinkRepository MagicLink { get; }

        IDbContextTransaction BeginTransaction();

        Task SaveAsync();
    }
}
