using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.Entities;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface IJwtService
    {
        Task<TokenDto> CreateTokenPairAsync(Guid userId, RequestIdentificationDto requestIdentificationDto);

        Task<TokenDto> CreateTokenPairAsync(TenantUser tenantUser, RequestIdentificationDto requestIdentificationDto);

        TokenUserInformationDto GetUserInformationFromToken(string token, bool allowExpired);
    }
}
