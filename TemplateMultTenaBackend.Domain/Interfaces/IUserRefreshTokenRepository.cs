using TemplateMultTenaBackend.Domain.Entities;

namespace TemplateMultTenaBackend.Domain.Interfaces
{
    public interface IUserRefreshTokenRepository
    {
        Task<UserRefreshToken?> GetByTokenAsync(User user, string token, bool trackChanges);

        void Create(UserRefreshToken userRefreshToken);

        void Update(UserRefreshToken userRefreshToken);
    }
}
