using MediatR;
using Microsoft.AspNetCore.Identity;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Tenant;
using TemplateMultTenaBackend.Domain.DataTransferObjects.User;
using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.Entities.Enums;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.Interfaces;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;


        public TenantService(
            IRepositoryManager repository,
            UserManager<User> userManager,
            IJwtService jwtService)
        {
            _repository = repository;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<ServiceResult<TokenDto>> CreateAsync(
            TenantForCreationDto tenantDto,
            Guid ownerId,
            RequestIdentificationDto requestIdentificationDto)
        {
            using var transaction = _repository.BeginTransaction();
            try
            {
                var tenantUser = await CreateWithoutTransactionAsync(tenantDto, ownerId);
                var token = await _jwtService.CreateTokenPairAsync(tenantUser, requestIdentificationDto);

                await transaction.CommitAsync();
                return ServiceResult<TokenDto>.Success(token);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<ServiceResult<TenantDto>> GetByIdAsync(Guid userId, Guid tenantId, bool trackChanges)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var tenants = await _repository.Tenant.GetAllForUserAsync(userId, includeOwner: true);
            Tenant? tenant = tenants.FirstOrDefault(t => t.Id == tenantId);

            if (tenant is null)
                return ServiceResult<TenantDto>.Failure(
                    [$"Tenant com Id {tenantId} não encontrado, ou você não tem permissão para acessá-lo."],
                    404);

            var tenantDto = new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Owner = tenant.Owner is null ? null : new UserDto
                {
                    Id = tenant.Owner.Id,
                    FirstName = tenant.Owner.FirstName,
                    LastName = tenant.Owner.LastName,
                    Email = tenant.Owner.Email
                }
            };

            return ServiceResult<TenantDto>.Success(tenantDto);
        }

        public async Task<ServiceResult<IEnumerable<TenantDto>>> GetAllForUserAsync(Guid userId)
        {
            var tenants = await _repository.Tenant.GetAllForUserAsync(userId);

            var tenantDtos = tenants.Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Owner = t.Owner is null ? null : new UserDto
                {
                    Id = t.Owner.Id,
                    FirstName = t.Owner.FirstName,
                    LastName = t.Owner.LastName,
                    Email = t.Owner.Email
                }
            });

            return ServiceResult<IEnumerable<TenantDto>>.Success(tenantDtos);
        }


        public async Task<ServiceResult<Unit>> UpdateAsync(Guid userId, Guid tenantId, TenantForUpdateDto tenantDto, bool trackChanges)
        {
            var tenant = await _repository.Tenant.GetByIdAsync(tenantId, trackChanges);

            if (tenant is null)
                return ServiceResult<Unit>.Failure([$"Tenant com Id {tenantId} não encontrado, ou você não tem permissão para acessá-lo."], 404);

            if (tenantDto.OwnerId != tenant.OwnerId && tenant.OwnerId != userId)
                return ServiceResult<Unit>.Failure(["Apenas o proprietário da conta pode transferi-la para outro dono."], 403);

            var ownerTenantUser = await _repository.TenantUser.GetByTenantAndUserAsync(tenant.Id, tenantDto.OwnerId, false, false, false);

            if (ownerTenantUser is null)
                return ServiceResult<Unit>.Failure(["O novo dono do tenant precisa estar no tenant."], 422);

            if (ownerTenantUser.RoleId != Roles.Admininstrator)
                return ServiceResult<Unit>.Failure(["O novo dono do Tenant precisa ser um administrador."], 422);

            tenant.Name = tenantDto.Name;
            tenant.OwnerId = tenantDto.OwnerId;
            tenant.UpdatedAt = DateTime.UtcNow;

            _repository.Tenant.Update(tenant);
            await _repository.SaveAsync();

            return ServiceResult<Unit>.Success();
        }

        private async Task<TenantUser> CreateWithoutTransactionAsync(TenantForCreationDto tenantDto, Guid ownerId)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = tenantDto.Name,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            User? user = await _userManager.FindByIdAsync(ownerId.ToString());
            tenant.Owner = user!;

            _repository.Tenant.Create(tenant);

            var currentDateTime = DateTime.UtcNow;
            var tenantUser = new TenantUser
            {
                TenantId = tenant.Id,
                UserId = ownerId,
                RoleId = Roles.Admininstrator,
                CreatedAt = currentDateTime,
                CreatedById = ownerId,
                UpdatedAt = currentDateTime,
                UpdatedById = ownerId
            };
            _repository.TenantUser.Create(tenantUser);

            await _repository.SaveAsync();

            return tenantUser!;
        }
    }
}
