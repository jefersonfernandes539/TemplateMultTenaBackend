using Microsoft.EntityFrameworkCore;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Infrastructure.Persistence;

namespace TemplateMultTenaBackend.Application.Repository
{
    public class UserRefreshTokenRepository : RepositoryBase<UserRefreshToken>, IUserRefreshTokenRepository
    {
        public UserRefreshTokenRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public void Create(UserRefreshToken userRefreshToken) => Add(userRefreshToken);

        public async Task<UserRefreshToken?> GetByTokenAsync(User user, string token, bool trackChanges)
        {
            var limitDate = DateTime.UtcNow.AddDays(-1 * UserRefreshToken.ValidForDays);
            return await FindByCondition(r => r.RevokedAt == null && r.CreatedAt >= limitDate && r.Token == token && r.UserId == user.Id, trackChanges)
                .SingleOrDefaultAsync();
        }
    }
}
