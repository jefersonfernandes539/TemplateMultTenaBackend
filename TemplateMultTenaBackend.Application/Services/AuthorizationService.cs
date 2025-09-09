using MediatR;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.Interfaces;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRepositoryManager _repository;

        public AuthorizationService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public static bool IsUserInRole(TenantUser tenantUser, Guid roleId) => tenantUser.RoleId == roleId;

        public static bool IsUserInRole(TenantUser tenantUser, IEnumerable<Guid> roleIds) => roleIds.Contains(tenantUser.RoleId);

        public async Task<ServiceResult<bool>> IsUserInRoleAsync(Guid tenantId, Guid userId, Guid roleId)
        {
            var tenantUser = await _repository.TenantUser.GetByTenantAndUserAsync(tenantId, userId, false, false, false);

            if (tenantUser is null)
                return ServiceResult<bool>.Failure(["Tenant/Usuário não encontrado ou você não tem permissão para ver este Tenant."], statusCode: 404);

            return ServiceResult<bool>.Success(roleId == tenantUser.RoleId);
        }

        public async Task<ServiceResult<bool>> IsUserInRoleAsync(Guid tenantId, Guid userId, IEnumerable<Guid> roleIds)
        {
            var tenantUser = await _repository.TenantUser.GetByTenantAndUserAsync(tenantId, userId, false, false, false);

            if (tenantUser is null)
                return ServiceResult<bool>.Failure(["Tenant/Usuário não encontrado ou você não tem permissão para ver este Tenant."], statusCode: 404);

            return ServiceResult<bool>.Success(roleIds.Contains(tenantUser.RoleId));
        }

        public async Task<ServiceResult<Unit>> AuthorizeAsync(Guid tenantId, Guid userId, Guid roleId)
        {
            if (tenantId == Guid.Empty)
                return ServiceResult<Unit>.Failure(["Você deve estar logado em um tenant para fazer isso."], 422);

            var isUserInRole = await IsUserInRoleAsync(tenantId, userId, roleId);

            if (!isUserInRole.Data)
                return ServiceResult<Unit>.Failure(["Você não tem permissão para fazer isso."], 403);

            return ServiceResult<Unit>.Success();
        }

        public async Task<ServiceResult<Unit>> AuthorizeAsync(Guid tenantId, Guid userId, IEnumerable<Guid> roleIds)
        {
            if (tenantId == Guid.Empty)
                return ServiceResult<Unit>.Failure(["Você deve estar logado em um tenant para fazer isso."], 422);

            var isUserInRole = await IsUserInRoleAsync(tenantId, userId, roleIds);

            if (!isUserInRole.Data)
                return ServiceResult<Unit>.Failure(["Você não tem permissão para fazer isso."], 403);

            return ServiceResult<Unit>.Success();
        }

        public async Task<ServiceResult<Unit>> EnsureTenantUserNotOwnerOrHimselfAsync(Guid tenantId, Guid userPerformingActionId, TenantUser tenantUserBeingManipulated)
        {
            var currentTenantUser = await _repository.TenantUser.GetByTenantAndUserAsync(
                tenantId,
                userPerformingActionId,
                false, false, false);

            if (currentTenantUser is null)
                return ServiceResult<Unit>.Failure([$"Usuário com ID {userPerformingActionId} não encontrado no seu Tenant atual."], 404);

            if (currentTenantUser.Id == tenantUserBeingManipulated.Id)
                return ServiceResult<Unit>.Failure([$"Você não pode alterar sua própria conta."], 403);

            var tenant = await _repository.Tenant.GetByIdAsync(tenantId, false);

            if (tenant is null)
                return ServiceResult<Unit>.Failure([$"Tenant com Id {tenantId} não encontrado, ou você não tem permissão para acessá-lo."], 404);

            if (tenant.OwnerId == tenantUserBeingManipulated.UserId)
                return ServiceResult<Unit>.Failure([$"Você não pode alterar o dono da conta."], 403);

            return ServiceResult<Unit>.Success();
        }
    }
}
