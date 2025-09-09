using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Domain.Interfaces
{
    public interface ITenantUserRepository
    {
        Task<PagedList<TenantUser>> GetAllForTenantAsync(Guid tenantId, TenantUserParameters tenantUserParameters, bool includeUserInfo, bool trackChanges);

        Task<TenantUser?> GetByIdAsync(Guid tenantId, Guid tenantUserId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges);

        Task<TenantUser?> GetByTenantAndUserAsync(Guid tenantId, Guid userId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges);

        Task<TenantUser?> GetDefaultForUser(Guid userId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges);

        Task<List<TenantUser>> GetAllForUserAsync(Guid userId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges);

        void Create(TenantUser tenantUser);

        void Update(TenantUser tenantUser);

        void Delete(TenantUser tenantUser);
    }
}
