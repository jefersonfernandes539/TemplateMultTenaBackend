using TemplateMultTenaBackend.Domain.Entities;

namespace TemplateMultTenaBackend.Domain.Interfaces
{
    public interface IMagicLinkRepository
    {
        void Create(MagicLink entity);

        void Update(MagicLink tenant);

        Task<MagicLink?> GetByIdAsync(Guid id, bool trackChanges);
    }
}
