using Microsoft.EntityFrameworkCore.Storage;
using TemplateMultTenaBackend.Domain.Interfaces;
using TemplateMultTenaBackend.Infrastructure.Persistence;

namespace TemplateMultTenaBackend.Infrastructure.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly ApplicationDbContext _applicationDbContext;

        // We use Lazy to avoid instatiating Repositories that are not being used.
        private readonly Lazy<ITenantRepository> _tenantRepository;

        private readonly Lazy<ITenantUserRepository> _tenantUserRepository;
        private readonly Lazy<IUserRefreshTokenRepository> _userRefreshTokenRepository;
        private readonly Lazy<IMagicLinkRepository> _magicLinkRepository;

        public RepositoryManager(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _tenantRepository = new Lazy<ITenantRepository>(() => new TenantRepository(applicationDbContext));
            _tenantUserRepository = new Lazy<ITenantUserRepository>(() => new TenantUserRepository(applicationDbContext));
            _userRefreshTokenRepository = new Lazy<IUserRefreshTokenRepository>(() => new UserRefreshTokenRepository(applicationDbContext));
            _magicLinkRepository = new Lazy<IMagicLinkRepository>(() => new MagicLinkRepository(applicationDbContext));
        }

        public ITenantRepository Tenant => _tenantRepository.Value;
        public ITenantUserRepository TenantUser => _tenantUserRepository.Value;
        public IUserRefreshTokenRepository UserRefreshToken => _userRefreshTokenRepository.Value;
        public IMagicLinkRepository MagicLink => _magicLinkRepository.Value;

        public IDbContextTransaction BeginTransaction() => _applicationDbContext.Database.BeginTransaction();

        public async Task SaveAsync() => await _applicationDbContext.SaveChangesAsync();
    }
}
