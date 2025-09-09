using MediatR;
using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Tenant;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface ITenantService
    {
        Task<ServiceResult<TenantDto>> GetByIdAsync(Guid userId, Guid tenantId, bool trackChanges);

        Task<ServiceResult<IEnumerable<TenantDto>>> GetAllForUserAsync(Guid userId);

        Task<ServiceResult<TokenDto>> CreateAsync(TenantForCreationDto tenantDto, Guid ownerId, RequestIdentificationDto requestIdentificationDto);

        Task<ServiceResult<Unit>> UpdateAsync(Guid userId, Guid tenantId, TenantForUpdateDto tenantDto, bool trackChanges);
    }
}
