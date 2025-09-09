using MediatR;
using TemplateMultTenaBackend.Domain.DataTransferObjects.TenantUser;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface ITenantUserService
    {
        Task<ServiceResult<PagedResponse<TenantUserDto>>> GetAllForTenantAsync(Guid tenantId, TenantUserParameters tenantUserParameters, bool includeUserInfo, bool trackChanges);

        Task<ServiceResult<TenantUserDto>> CreateAsync(TenantUserForCreationDto tenantUserDto, Guid currentUserId, Guid tenantId);

        Task<ServiceResult<TenantUserDto>> GetByIdAsync(Guid tenantId, Guid tenantUserId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges);

        Task<ServiceResult<TenantUserDto>> GetByTenantIdAndUserIdAsync(Guid tenantId, Guid userId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges);

        Task<ServiceResult<List<TenantUserDto>>> GetAllTenantUserByUserId(Guid userId, bool includeUserInfo, bool includeTenantInfo, bool trackChanges);

        Task<ServiceResult<Unit>> UpdateAsync(Guid tenantId, Guid tenantUserId, TenantUserForUpdateDto tenantUserDto, bool trackChanges, Guid currentUserId);

        Task<ServiceResult<Unit>> DeleteAsync(Guid tenantId, Guid currentUserId, Guid tenantUserToDeleteId, bool trackChanges);

        Task<ServiceResult<Unit>> LeaveTenant(Guid tenantId, Guid currentUserId, bool trackChanges);
    }
}
