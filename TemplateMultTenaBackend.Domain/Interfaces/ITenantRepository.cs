using TemplateMultTenaBackend.Domain.Entities;

namespace TemplateMultTenaBackend.Domain.Interfaces
{
    public interface ITenantRepository
    {
        Task<int> CountAllForUserAsync(Guid userId);

        Task<IEnumerable<Tenant>> GetAllForUserAsync(Guid userId, bool includeOwner = false);

        Task<Tenant?> GetByIdAsync(Guid id, bool trackChanges);

        void Create(Tenant tenant);

        void Update(Tenant tenant);
    }
}
