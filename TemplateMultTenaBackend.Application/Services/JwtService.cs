using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.ConfigurationModels;
using TemplateMultTenaBackend.Domain.DataTransferObjects;
using TemplateMultTenaBackend.Domain.DataTransferObjects.Authentication;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.Exceptions.Unauthorized;
using TemplateMultTenaBackend.Domain.Interfaces;

namespace TemplateMultTenaBackend.Application.Services
{
    public sealed class JwtService : IJwtService
    {
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IUserRefreshTokenService _userRefreshTokenService;
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;

        private SymmetricSecurityKey jwtSecret() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret!));

        public JwtService(IOptionsMonitor<JwtConfiguration> jwtConfiguration, IUserRefreshTokenService userRefreshTokenService,
            IRepositoryManager repository, UserManager<User> userManager)
        {
            _jwtConfiguration = jwtConfiguration.CurrentValue;
            _userRefreshTokenService = userRefreshTokenService;
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<TokenDto> CreateTokenPairAsync(Guid userId, RequestIdentificationDto requestIdentificationDto)
        {
            var refreshToken = await _userRefreshTokenService.GenerateAsync(userId, requestIdentificationDto);
            var tenantUser = await _repository.TenantUser.GetDefaultForUser(userId, false, false, false);
            var accessToken = (tenantUser is null) ? await CreateAccessTokenAsync(userId) : await CreateAccessTokenAsync(tenantUser);

            return new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<TokenDto> CreateTokenPairAsync(TenantUser tenantUser, RequestIdentificationDto requestIdentificationDto)
        {
            var refreshToken = await _userRefreshTokenService.GenerateAsync(tenantUser.UserId, requestIdentificationDto);
            var accessToken = await CreateAccessTokenAsync(tenantUser);

            return new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public TokenUserInformationDto GetUserInformationFromToken(string token, bool allowExpired)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtSecret(),
                ValidateLifetime = !allowExpired,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidJwtSecretUnauthorizedException();

            return new TokenUserInformationDto(principal);
        }

        public async Task<string> CreateAccessTokenAsync(Guid userId)
        {
            var signingCredentials = new SigningCredentials(jwtSecret(), SecurityAlgorithms.HmacSha256);
            var claims = await GetClaimsAsync(userId, null, null, null);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<string> CreateAccessTokenAsync(TenantUser tenantUser)
        {
            var signingCredentials = new SigningCredentials(jwtSecret(), SecurityAlgorithms.HmacSha256);
            var claims = await GetClaimsAsync(tenantUser.UserId, tenantUser.TenantId, tenantUser.RoleId, tenantUser.Id);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetClaimsAsync(Guid userId, Guid? tenantId, Guid? roleId, Guid? tenantUserId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, userId.ToString())
            };

            if (user is not null && user.FirstName is not null)
                claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));

            if (user is not null && user.LastName is not null)
                claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

            if (tenantId is not null && tenantId != Guid.Empty)
                claims.Add(new Claim("Tenant.Id", tenantId.ToString()!));

            if (roleId is not null && roleId != Guid.Empty)
                claims.Add(new Claim("TenantUser.RoleId", roleId.ToString()!));

            if (tenantUserId is not null && tenantUserId != Guid.Empty)
                claims.Add(new Claim("TenantUser.Id", tenantUserId.ToString()!));

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(int.Parse(_jwtConfiguration.Expires!)),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }
    }
}
