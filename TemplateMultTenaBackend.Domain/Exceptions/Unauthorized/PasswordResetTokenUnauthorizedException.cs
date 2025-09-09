namespace TemplateMultTenaBackend.Domain.Exceptions.Unauthorized
{
    public sealed class PasswordResetTokenUnauthorizedException : UnauthorizedException
    {
        public PasswordResetTokenUnauthorizedException() : base("Link expirado.")
        {
        }
    }
}
