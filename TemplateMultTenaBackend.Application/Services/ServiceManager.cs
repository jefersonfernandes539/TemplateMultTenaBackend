using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TemplateMultTenaBackend.Application.Interfaces;
using TemplateMultTenaBackend.Domain.ConfigurationModels;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Domain.Interfaces;

namespace TemplateMultTenaBackend.Application.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IAuthorizationService> _authorizationService;
        private readonly Lazy<IJwtService> _jwtService;
        private readonly Lazy<ITenantService> _tenantService;
        private readonly Lazy<ITenantUserService> _tenantUserService;
        private readonly Lazy<IUserRefreshTokenService> _userRefreshTokenService;
        private readonly Lazy<RoleManager<Role>> _roleManager;
        private readonly Lazy<IMagicLinkService> _magicLinkService;
        private readonly Lazy<IUserService> _userService;

        public ServiceManager(UserManager<User> userManager, IRepositoryManager repository,
            IConfiguration configuration, RoleManager<Role> roleManager, IOptionsMonitor<JwtConfiguration> jwtConfiguration, IPasswordValidator<User> passwordValidator, IHttpClientFactory httpClientFactory)
        {
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager, repository, UserRefreshTokenService, JwtService, configuration, passwordValidator));
            _authorizationService = new Lazy<IAuthorizationService>(() => new AuthorizationService(repository));
            _jwtService = new Lazy<IJwtService>(() => new JwtService(jwtConfiguration, UserRefreshTokenService, repository, userManager));
            _tenantService = new Lazy<ITenantService>(() => new TenantService(repository, userManager, JwtService));
            _tenantUserService = new Lazy<ITenantUserService>(() => new TenantUserService(repository, AuthorizationService, userManager, MagicLinkService, configuration));
            _userRefreshTokenService = new Lazy<IUserRefreshTokenService>(() => new UserRefreshTokenService(repository));
            _roleManager = new Lazy<RoleManager<Role>>(() => roleManager);
            _magicLinkService = new Lazy<IMagicLinkService>(() => new MagicLinkService(userManager, repository, JwtService));
            _userService = new Lazy<IUserService>(() => new UserService(repository, userManager, JwtService));
        }

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IAuthorizationService AuthorizationService => _authorizationService.Value;
        public IJwtService JwtService => _jwtService.Value;
        public ITenantService TenantService => _tenantService.Value;
        public ITenantUserService TenantUserService => _tenantUserService.Value;
        public IUserRefreshTokenService UserRefreshTokenService => _userRefreshTokenService.Value;
        public RoleManager<Role> RoleManager => _roleManager.Value;
        public IMagicLinkService MagicLinkService => _magicLinkService.Value;
        public IUserService UserService => _userService.Value;
    }
}
