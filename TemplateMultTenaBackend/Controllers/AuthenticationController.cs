using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.DataTransferObjects.User;
using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.RequestFeatures.Extensions;
using Web.ActionFilters;

namespace Web.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : MainController
    {
        private readonly IServiceManager _service;
        private readonly RequestIdentificationDto _requestIdentificationDto;

        public AuthenticationController(IHttpContextAccessor httpContextAccessor, IServiceManager service)
        {
            _service = service;

            var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

            _requestIdentificationDto = new RequestIdentificationDto() { IpAddress = ipAddress, UserAgent = userAgent };
        }

        [HttpPost("register")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var result = await _service.AuthenticationService.RegisterAsync(userForRegistration);
            return CustomResponse(result);
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            var result = await _service.AuthenticationService.ValidateAndReturnTokenAsync(user, _requestIdentificationDto);
            return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, result.ErrorMessages);
        }

        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenForRefreshDto tokenDto)
        {
            var tokenDtoToReturn = await _service.AuthenticationService.RefreshTokenAsync(tokenDto, _requestIdentificationDto);
            return Ok(tokenDtoToReturn);
        }

        [HttpGet("magic/{magicLinkId:guid}")]
        public async Task<IActionResult> GenerateTokenByMagicLinkId(Guid magicLinkId)
        {
            var tokenDto = await _service.MagicLinkService.GenerateTokenByMagicLinkId(magicLinkId, _requestIdentificationDto);
            return tokenDto.IsSuccess ? Ok(tokenDto.Data) : StatusCode(tokenDto.StatusCode, tokenDto.ErrorMessages);
        }

        [HttpPost("reset")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RequestPasswordReset([FromBody] UserForPasswordResetDto dto)
        {
            await _service.AuthenticationService.RequestPasswordResetAsync(dto);
            return Accepted();
        }

        [HttpPut("reset")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
        {
            await _service.AuthenticationService.PasswordResetAsync(dto);
            return Ok();
        }

        [HttpPut("complete-signin")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Authorize]
        public async Task<IActionResult> CompleteSignin([FromBody] UserForCompleteSigninDto dto)
        {
            var tenantIdFromToken = HttpContext?.User?.FindFirst("Tenant.Id")?.Value;
            var tenantId = Guid.TryParse(tenantIdFromToken, out var tenantGuid) ? tenantGuid : Guid.Empty;

            var tenantUserIdFromToken = HttpContext?.User?.FindFirst("TenantUser.Id")?.Value;
            var tenantUserId = Guid.TryParse(tenantUserIdFromToken, out var currentUserIdGuid) ? currentUserIdGuid : Guid.Empty;

            var token = await _service.AuthenticationService.CompleteSignin(dto, tenantId, tenantUserId, _requestIdentificationDto);
            return Ok(token);
        }
    }
}