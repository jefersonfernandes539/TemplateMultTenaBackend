using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.DataTransferObjects.TenantUser;
using TemplateMultTenaBackend.Domain.DataTransferObjects.User;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.Entities.Enums;
using TemplateMultTenaBackend.Domain.Exceptions.NotFound;
using TemplateMultTenaBackend.Domain.Interfaces;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Services
{
    public class TenantUserService : ITenantUserService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMagicLinkService _magicLinkService;
        private readonly IConfiguration _configuration;

        public TenantUserService(
            IRepositoryManager repository,
            IAuthorizationService authorizationService,
            UserManager<User> userManager,
            IMagicLinkService magicLinkService,
            IConfiguration configuration)
        {
            _repository = repository;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _magicLinkService = magicLinkService;
            _configuration = configuration;
        }

        #region Get Methods

        public async Task<ServiceResult<PagedResponse<TenantUserDto>>> GetAllForTenantAsync(
            Guid tenantId,
            TenantUserParameters tenantUserParameters,
            bool includeUserInfo,
            bool trackChanges)
        {
            if (tenantUserParameters.RoleId != Guid.Empty && !Roles.AnyRole.Contains(tenantUserParameters.RoleId))
                return ServiceResult<PagedResponse<TenantUserDto>>.Failure([$"Cargo inválido"], 422);

            var tenantUsers = await _repository.TenantUser.GetAllForTenantAsync(tenantId, tenantUserParameters, includeUserInfo, trackChanges);

            var dtos = tenantUsers.Select(t => MapToDto(t)).ToList();
            var pagedResponse = new PagedResponse<TenantUserDto>(dtos, tenantUsers.MetaData);

            return ServiceResult<PagedResponse<TenantUserDto>>.Success(pagedResponse);
        }

        public async Task<ServiceResult<TenantUserDto>> GetByIdAsync(
            Guid tenantId,
            Guid tenantUserId,
            bool includeUserInfo,
            bool includeTenantInfo,
            bool trackChanges)
        {
            var tenantUserResult = await GetMandatory(tenantId, tenantUserId, includeUserInfo, includeTenantInfo, trackChanges);
            if (!tenantUserResult.IsSuccess)
                return ServiceResult<TenantUserDto>.Failure(tenantUserResult.ErrorMessages, tenantUserResult.StatusCode);

            return ServiceResult<TenantUserDto>.Success(MapToDto(tenantUserResult.Data!));
        }

        public async Task<ServiceResult<TenantUserDto>> GetByTenantIdAndUserIdAsync(
            Guid tenantId,
            Guid userId,
            bool includeUserInfo,
            bool includeTenantInfo,
            bool trackChanges)
        {
            var tenantUserResult = await GetMandatoryWithUserId(tenantId, userId, includeUserInfo, includeTenantInfo, trackChanges);
            if (!tenantUserResult.IsSuccess)
                return ServiceResult<TenantUserDto>.Failure(tenantUserResult.ErrorMessages, tenantUserResult.StatusCode);

            return ServiceResult<TenantUserDto>.Success(MapToDto(tenantUserResult.Data!));
        }

        public async Task<ServiceResult<List<TenantUserDto>>> GetAllTenantUserByUserId(
            Guid userId,
            bool includeUserInfo,
            bool includeTenantInfo,
            bool trackChanges)
        {
            var tenantUsers = await _repository.TenantUser.GetAllForUserAsync(userId, includeUserInfo, includeTenantInfo, trackChanges);
            var dtos = tenantUsers.Select(t => MapToDto(t)).ToList();

            return ServiceResult<List<TenantUserDto>>.Success(dtos);
        }

        #endregion

        #region Create / Update / Delete

        public async Task<ServiceResult<TenantUserDto>> CreateAsync(
            TenantUserForCreationDto tenantUserDto,
            Guid currentUserId,
            Guid tenantId)
        {
            using var transaction = _repository.BeginTransaction();
            try
            {
                var user = await _userManager.FindByEmailAsync(tenantUserDto.Email);
                if (user is null)
                {
                    var createdUser = await CreateUser(tenantUserDto.Email, tenantUserDto.PhoneNumber);
                    if (!createdUser.IsSuccess)
                        return ServiceResult<TenantUserDto>.Failure(createdUser.ErrorMessages, createdUser.StatusCode);

                    user = createdUser.Data!;
                }

                var existingTenantUser = await _repository.TenantUser.GetByTenantAndUserAsync(tenantId, user.Id, false, false, false);
                if (existingTenantUser != null)
                    return ServiceResult<TenantUserDto>.Failure(["Este usuário já está cadastrado."], 422);

                var tenant = await _repository.Tenant.GetByIdAsync(tenantId, false);
                if (tenant is null)
                    return ServiceResult<TenantUserDto>.Failure([$"Tenant com Id {tenantId} não encontrado."], 404);

                var entity = new TenantUser
                {
                    CreatedById = currentUserId,
                    UpdatedById = currentUserId,
                    UserId = user.Id,
                    TenantId = tenantId,
                    RoleId = tenantUserDto.RoleId,
                };

                _repository.TenantUser.Create(entity);
                await _repository.SaveAsync();
                await transaction.CommitAsync();

                return ServiceResult<TenantUserDto>.Success(MapToDto(entity, user));
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<ServiceResult<Unit>> UpdateAsync(
            Guid tenantId,
            Guid tenantUserId,
            TenantUserForUpdateDto tenantUserDto,
            bool trackChanges,
            Guid currentUserId)
        {
            var tenantUserResult = await GetMandatory(tenantId, tenantUserId, false, true, trackChanges);
            if (!tenantUserResult.IsSuccess)
                return ServiceResult<Unit>.Failure(tenantUserResult.ErrorMessages, tenantUserResult.StatusCode);

            var authorization = await _authorizationService.EnsureTenantUserNotOwnerOrHimselfAsync(tenantId, currentUserId, tenantUserResult.Data!);
            if (!authorization.IsSuccess)
                return ServiceResult<Unit>.Failure(authorization.ErrorMessages, authorization.StatusCode);

            var tenantUser = tenantUserResult.Data!;
            tenantUser.RoleId = tenantUserDto.RoleId;
            tenantUser.UpdatedAt = DateTime.UtcNow;
            tenantUser.UpdatedById = currentUserId;

            _repository.TenantUser.Update(tenantUser);
            await _repository.SaveAsync();

            return ServiceResult<Unit>.Success();
        }

        public async Task<ServiceResult<Unit>> DeleteAsync(
            Guid tenantId,
            Guid currentUserId,
            Guid tenantUserToDeleteId,
            bool trackChanges)
        {
            var tenantUserToDelete = await _repository.TenantUser.GetByIdAsync(tenantId, tenantUserToDeleteId, false, false, trackChanges);
            if (tenantUserToDelete is null)
                return ServiceResult<Unit>.Failure([$"Nenhum usuário com {tenantUserToDeleteId} foi encontrado no Tenant."], 404);

            var authorization = await _authorizationService.EnsureTenantUserNotOwnerOrHimselfAsync(tenantId, currentUserId, tenantUserToDelete);
            if (!authorization.IsSuccess)
                return ServiceResult<Unit>.Failure(authorization.ErrorMessages, authorization.StatusCode);

            _repository.TenantUser.Delete(tenantUserToDelete);
            await _repository.SaveAsync();
            return ServiceResult<Unit>.Success();
        }

        public async Task<ServiceResult<Unit>> LeaveTenant(Guid tenantId, Guid currentUserId, bool trackChanges)
        {
            var tenantUserResult = await GetMandatoryWithUserId(tenantId, currentUserId, false, false, trackChanges);
            if (!tenantUserResult.IsSuccess)
                return ServiceResult<Unit>.Failure(tenantUserResult.ErrorMessages, tenantUserResult.StatusCode);

            var canLeave = await CanLeaveTenant(tenantUserResult.Data!);
            if (!canLeave.IsSuccess)
                return ServiceResult<Unit>.Failure(canLeave.ErrorMessages, canLeave.StatusCode);

            _repository.TenantUser.Delete(tenantUserResult.Data!);
            await _repository.SaveAsync();

            return ServiceResult<Unit>.Success();
        }

        #endregion

        #region Helpers

        private TenantUserDto MapToDto(TenantUser tenantUser, User? user = null)
        {
            return new TenantUserDto
            {
                Id = tenantUser.Id,
                TenantId = tenantUser.TenantId,
                UserId = tenantUser.UserId,
                RoleId = tenantUser.RoleId,
                CreatedAt = tenantUser.CreatedAt,
                UpdatedAt = tenantUser.UpdatedAt,
                User = (user ?? tenantUser.User) is null ? null : new UserDto
                {
                    Id = (user ?? tenantUser.User)!.Id,
                    FirstName = (user ?? tenantUser.User)!.FirstName,
                    LastName = (user ?? tenantUser.User)!.LastName,
                    Email = (user ?? tenantUser.User)!.Email
                }
            };
        }

        private async Task<ServiceResult<User>> CreateUser(string email, string phoneNumber)
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = email,
                PhoneNumber = phoneNumber,
                UserName = userId.ToString(),
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return ServiceResult<User>.Failure(["Erro ao criar usuário."], 422);

            return ServiceResult<User>.Success(user);
        }

        private async Task<ServiceResult<TenantUser>> GetMandatory(
            Guid tenantId,
            Guid tenantUserId,
            bool includeUserInfo,
            bool includeTenantInfo,
            bool trackChanges)
        {
            var tenantUser = await _repository.TenantUser.GetByIdAsync(tenantId, tenantUserId, includeUserInfo, includeTenantInfo, trackChanges);
            if (tenantUser == null)
                return ServiceResult<TenantUser>.Failure([$"Nenhum usuário com {tenantUserId} foi encontrado no Tenant."], 404);

            return ServiceResult<TenantUser>.Success(tenantUser);
        }

        private async Task<ServiceResult<TenantUser>> GetMandatoryWithUserId(
            Guid tenantId,
            Guid userId,
            bool includeUserInfo,
            bool includeTenantInfo,
            bool trackChanges)
        {
            var tenantUser = await _repository.TenantUser.GetByTenantAndUserAsync(tenantId, userId, includeUserInfo, includeTenantInfo, trackChanges);
            if (tenantUser == null)
                return ServiceResult<TenantUser>.Failure([$"Nenhum usuário com {userId} foi encontrado no Tenant."], 404);

            return ServiceResult<TenantUser>.Success(tenantUser);
        }

        private async Task<ServiceResult<Unit>> CanLeaveTenant(TenantUser tenantUser)
        {
            var tenant = await _repository.Tenant.GetByIdAsync(tenantUser.TenantId, false) ?? throw new TenantNotFoundException(tenantUser.TenantId);

            if (tenant.OwnerId == tenantUser.UserId)
                return ServiceResult<Unit>.Failure(["Você não pode sair do Tenant sendo o dono."], 403);

            var tenants = await _repository.Tenant.CountAllForUserAsync(tenantUser.UserId);
            if (tenants <= 1)
                return ServiceResult<Unit>.Failure(["Usuário deve ter ao menos uma organização."], 422);

            return ServiceResult<Unit>.Success();
        }

        #endregion
    }
}
