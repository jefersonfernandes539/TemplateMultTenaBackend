using Microsoft.EntityFrameworkCore;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Infrastructure.Persistence;

namespace TemplateMultTenaBackend.Application.Repository
{
    public class MagicLinkRepository : RepositoryBase<MagicLink>, IMagicLinkRepository
    {
        public MagicLinkRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public void Create(MagicLink entity) => Add(entity);

        public async Task<MagicLink?> GetByIdAsync(Guid id, bool trackChanges)
        {
            return await FindByCondition(t => t.Id.Equals(id), trackChanges).Include(t => t.User).SingleOrDefaultAsync();
        }
    }
}
