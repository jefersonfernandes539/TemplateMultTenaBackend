namespace TemplateMultTenaBackend.Domain.Exceptions.Unauthorized
{
    public sealed class RefreshTokenUnauthorized : UnauthorizedException
    {
        public RefreshTokenUnauthorized() : base("Token ou RefreshToken inválidos.")
        {
        }
    }
}
