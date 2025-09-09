namespace TemplateMultTenaBackend.Domain.Exceptions.Unauthorized
{
    public sealed class InvalidJwtSecretUnauthorizedException : UnauthorizedAccessException
    {
        public InvalidJwtSecretUnauthorizedException() : base("Token JWT inválido.")
        {
        }
    }
}
