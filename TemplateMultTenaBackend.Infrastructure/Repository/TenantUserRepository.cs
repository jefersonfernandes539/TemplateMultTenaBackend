using Microsoft.EntityFrameworkCore;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.Interfaces;
using TemplateMultTenaBackend.Domain.RequestFeatures;
using TemplateMultTenaBackend.Infrastructure.Persistence;

namespace TemplateMultTenaBackend.Infrastructure.Repository
{
    public class TenantUserRepository : RepositoryBase<TenantUser>, ITenantUserRepository
    {
        public TenantUserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public void Create(TenantUser tenantUser) => Add(tenantUser);

        public void Delete(TenantUser tenantUser) => Remove(tenantUser);

        public async Task<PagedList<TenantUser>> GetAllForTenantAsync(Guid tenantId, TenantUserParameters tenantUserParameters, bool includeUserInfo, bool trackChanges)
        {
            var query = FindByCondition(tu => tu.TenantId.Equals(tenantId), trackChanges);

            if (tenantUserParameters.RoleId != Guid.Empty)
                query = query.Where(tu => tu.RoleId.Equals(tenantUserParameters.RoleId));

            if (includeUserInfo)
                query = query.Include(tu => tu.User).Include(tu => tu.CreatedBy).Include(tu => tu.UpdatedBy);

            return await PagedList<TenantUser>.ToPagedList(query, tenantUserParameters.PageNumber, tenantUserParameters.PageSize);
        }

        public async Task<TenantUser?> GetByIdAsync(Guid tenantId, Guid tenantUserId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges)
        {
            var query = FindByCondition(tu => tu.TenantId == tenantId && tu.Id.Equals(tenantUserId), trackChanges);

            if (includeUserInfo)
                query = query.Include(tu => tu.User);

            if (includeTenantInfo)
                query = query.Include(tu => tu.Tenant);

            return await query.SingleOrDefaultAsync();
        }

        public async Task<TenantUser?> GetByTenantAndUserAsync(Guid tenantId, Guid userId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges)
        {
            var query = FindByCondition(tu => tu.UserId.Equals(userId) && tu.TenantId.Equals(tenantId), trackChanges);

            if (includeUserInfo)
                query = query.Include(tu => tu.User);

            if (includeTenantInfo)
                query = query.Include(tu => tu.Tenant);

            return await query.SingleOrDefaultAsync();
        }

        public async Task<TenantUser?> GetDefaultForUser(Guid userId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges)
        {
            var query = FindByCondition(tu => tu.UserId.Equals(userId), trackChanges);

            if (includeUserInfo)
                query = query.Include(tu => tu.User);

            if (includeTenantInfo)
                query = query.Include(tu => tu.Tenant);

            return await query.OrderBy(e => e.CreatedAt).FirstOrDefaultAsync();
        }

        public async Task<List<TenantUser>> GetAllForUserAsync(Guid userId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges)
        {
            var query = FindByCondition(tu => tu.UserId.Equals(userId), trackChanges);

            if (includeUserInfo)
                query = query.Include(tu => tu.User);

            if (includeTenantInfo)
                query = query.Include(tu => tu.Tenant);

            return await query.ToListAsync();
        }
    }
}
