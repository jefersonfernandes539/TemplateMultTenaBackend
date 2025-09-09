using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.DataTransferObjects.User;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.Exceptions.NotFound;
using TemplateMultTenaBackend.Domain.Exceptions.Unauthorized;
using TemplateMultTenaBackend.Domain.Exceptions.Unprocessable;
using TemplateMultTenaBackend.Domain.Interfaces;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRefreshTokenService _userRefreshTokenService;
        private readonly IPasswordValidator<User> _passwordValidator;

        public AuthenticationService(
            UserManager<User> userManager,
            IRepositoryManager repository,
            IUserRefreshTokenService userRefreshTokenService,
            IJwtService jwtService,
            IConfiguration configuration,
            IPasswordValidator<User> passwordValidator)
        {
            _userManager = userManager;
            _repository = repository;
            _userRefreshTokenService = userRefreshTokenService;
            _jwtService = jwtService;
            _configuration = configuration;
            _passwordValidator = passwordValidator;
        }

        public async Task<ServiceResult<TokenDto>> RegisterAsync(UserForRegistrationDto userDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = Guid.NewGuid().ToString(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                var tokenDto = await _jwtService.CreateTokenPairAsync(user.Id, new RequestIdentificationDto());
                return ServiceResult<TokenDto>.Success(tokenDto);
            }

            return ServiceResult<TokenDto>.Failure(result.Errors.Select(e => e.Description), 422);
        }

        public async Task<ServiceResult<TokenDto>> ValidateAndReturnTokenAsync(UserForAuthenticationDto userForAuth, RequestIdentificationDto requestIdentificationDto)
        {
            var user = userForAuth.EmailOrPhoneNumber.Contains("@")
                ? await _userManager.FindByEmailAsync(userForAuth.EmailOrPhoneNumber)
                : await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == userForAuth.EmailOrPhoneNumber);

            if (user == null)
                return ServiceResult<TokenDto>.Failure(["Credenciais inválidas ou o usuário não existe."], 401);

            if (!await _userManager.CheckPasswordAsync(user, userForAuth.Password))
                return ServiceResult<TokenDto>.Failure(["Credenciais inválidas ou o usuário não existe."], 401);

            var tokenDto = await CreateTokenPair(userForAuth.TenantId, user!.Id, requestIdentificationDto);
            return ServiceResult<TokenDto>.Success(tokenDto);
        }

        public async Task<TokenDto> RefreshTokenAsync(TokenForRefreshDto tokenDto, RequestIdentificationDto requestIdentificationDto)
        {
            var userInfo = _jwtService.GetUserInformationFromToken(tokenDto.AccessToken, true);
            var refreshToken = await GetRefreshTokenMandatoryAsync(userInfo.UserId, tokenDto.RefreshToken);
            var tenantId = (tokenDto.TenantId == Guid.Empty) ? userInfo.TenantId : tokenDto.TenantId;

            await _userRefreshTokenService.RevokeAsync(refreshToken, requestIdentificationDto);

            return await CreateTokenPair(tenantId, userInfo.UserId, requestIdentificationDto);
        }

        public async Task PasswordResetAsync(PasswordResetDto passwordResetDto)
        {
            var user = await _userManager.FindByIdAsync(passwordResetDto.UserId.ToString())
                       ?? throw new PasswordResetTokenUnauthorizedException();

            await ValidateAndResetPassword(passwordResetDto.Token, user, passwordResetDto.NewPassword);
        }

        public async Task<TokenDto> CompleteSignin(UserForCompleteSigninDto dto, Guid tenantId, Guid tenantUserId, RequestIdentificationDto requestIdentificationDto)
        {
            using var transaction = _repository.BeginTransaction();
            try
            {
                var tenantUser = await _repository.TenantUser.GetByIdAsync(tenantId, tenantUserId, true, false, true)
                    ?? throw new TenantUserNotFoundException();
                var user = tenantUser!.User;

                if (user.FirstName != null && user.LastName != null && user.PasswordHash != null)
                    throw new UserCompleteSignInUnprocessableException();

                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new UserUpdateUnprocessableException(updateResult.Errors.First().Description);

                var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                await ValidateAndResetPassword(passwordToken, user, dto.NewPassword);

                await transaction.CommitAsync();

                return await CreateTokenPair(tenantId, user.Id, requestIdentificationDto);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<UserRefreshToken> GetRefreshTokenMandatoryAsync(Guid userId, string currentRefreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new RefreshTokenUnauthorized();
            var refreshToken = await _repository.UserRefreshToken.GetByTokenAsync(user, currentRefreshToken, true)
                ?? throw new RefreshTokenUnauthorized();

            return refreshToken;
        }

        private async Task<TenantUser?> AuthorizeTenantUser(Guid? tenantId, Guid userId)
        {
            if (tenantId == Guid.Empty || tenantId == null)
                return await _repository.TenantUser.GetDefaultForUser(userId, false, false, false);

            var tenantUser = await _repository.TenantUser.GetByTenantAndUserAsync((Guid)tenantId, userId, false, false, false)
                ?? throw new TenantUserNotFoundException();

            return tenantUser!;
        }

        private async Task<TokenDto> CreateTokenPair(Guid? tenantId, Guid userId, RequestIdentificationDto requestIdentificationDto)
        {
            var tenantUser = await AuthorizeTenantUser(tenantId, userId);
            if (tenantUser is null)
                return await _jwtService.CreateTokenPairAsync(userId, requestIdentificationDto);

            return await _jwtService.CreateTokenPairAsync(tenantUser, requestIdentificationDto);
        }

        private async Task ValidateAndResetPassword(string passwordToken, User user, string password)
        {
            var validation = await _passwordValidator.ValidateAsync(_userManager, user, password);
            if (!validation.Succeeded) throw new PasswordUnprocessableException();

            var passwordChangeResult = await _userManager.ResetPasswordAsync(user, passwordToken, password);
            if (!passwordChangeResult.Succeeded) throw new PasswordResetTokenUnauthorizedException();
        }

        public Task RequestPasswordResetAsync(UserForPasswordResetDto userForPasswordResetDto)
        {
            throw new NotImplementedException();
        }
    }
}
