using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface IMagicLinkService
    {
        Task<ServiceResult<MagicLinkDto>> CreateAsync(MagicLinkForCreationDto dto);

        Task<ServiceResult<TokenDto>> GenerateTokenByMagicLinkId(Guid magicLinkId, RequestIdentificationDto requestIdentificationDto);
    }
}
