using Microsoft.EntityFrameworkCore;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Infrastructure.Persistence;

namespace TemplateMultTenaBackend.Application.Repository
{
    public class TenantRepository : RepositoryBase<Tenant>, ITenantRepository
    {
        public TenantRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public void Create(Tenant tenant) => Add(tenant);

        public async Task<Tenant?> GetByIdAsync(Guid id, bool trackChanges)
        {
            return await FindByCondition(t => t.Id.Equals(id), trackChanges).Include(t => t.Owner).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Tenant>> GetAllForUserAsync(Guid userId, bool includeOwner = false)
        {
            var query = ApplicationDbContext.Set<TenantUser>()
                .Where(tu => tu.UserId.Equals(userId)).AsQueryable();

            query = includeOwner ?
                query
                    .Include(tu => tu.Tenant)
                    .ThenInclude(tu => tu.Owner)
                    .AsQueryable() :
                query
                    .Include(tu => tu.Tenant)
                    .AsQueryable();

            return await query.Select(tu => tu.Tenant).ToListAsync();
        }

        public async Task<int> CountAllForUserAsync(Guid userId)
        {
            return await ApplicationDbContext.Set<TenantUser>()
                .Where(tu => tu.UserId.Equals(userId))
                .CountAsync();
        }
    }
}
