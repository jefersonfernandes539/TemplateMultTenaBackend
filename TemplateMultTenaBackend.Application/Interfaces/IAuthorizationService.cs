using MediatR;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface IAuthorizationService
    {
        Task<ServiceResult<bool>> IsUserInRoleAsync(Guid tenantId, Guid userId, Guid roleId);

        Task<ServiceResult<bool>> IsUserInRoleAsync(Guid tenantId, Guid userId, IEnumerable<Guid> roleIds);

        Task<ServiceResult<Unit>> AuthorizeAsync(Guid tenantId, Guid userId, Guid roleId);

        Task<ServiceResult<Unit>> AuthorizeAsync(Guid tenantId, Guid userId, IEnumerable<Guid> roleIds);

        Task<ServiceResult<Unit>> EnsureTenantUserNotOwnerOrHimselfAsync(Guid tenantId, Guid userPerformingActionId, TenantUser tenantUserBeingManipulated);
    }
}
