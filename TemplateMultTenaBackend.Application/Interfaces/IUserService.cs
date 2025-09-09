using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.DataTransferObjects.User;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<TokenDto>> UpdateAsync(Guid currentUserId, Guid currentTenant, UserForUpdateDto dto, bool trackChanges, RequestIdentificationDto identificationDto);
    }
}
