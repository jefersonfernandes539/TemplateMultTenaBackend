using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.DataTransferObjects.User;
using TemplateMultTenaBackend.Domain.RequestFeatures;

namespace TemplateMultTenaBackend.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<TokenDto>> RegisterAsync(UserForRegistrationDto userForRegistration);

        Task<ServiceResult<TokenDto>> ValidateAndReturnTokenAsync(UserForAuthenticationDto userForAuth, RequestIdentificationDto requestIdentificationDto);

        Task<TokenDto> RefreshTokenAsync(TokenForRefreshDto tokenDto, RequestIdentificationDto requestIdentificationDto);

        Task RequestPasswordResetAsync(UserForPasswordResetDto userForPasswordResetDto);

        Task PasswordResetAsync(PasswordResetDto passwordResetDto);

        Task<TokenDto> CompleteSignin(UserForCompleteSigninDto dto, Guid tenantId, Guid tenantUserId, RequestIdentificationDto requestIdentificationDto);
    }
}
