using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.Entities;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface IUserRefreshTokenService
    {
        Task<string> GenerateAsync(Guid userId, RequestIdentificationDto requestIdentification);

        Task RevokeAsync(UserRefreshToken userRefreshToken, RequestIdentificationDto requestIdentification);
    }
}
