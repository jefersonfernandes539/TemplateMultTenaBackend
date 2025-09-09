namespace TemplateMultTenaBackend.Domain.Exceptions.NotFound
{
    public sealed class TenantNotFoundException : NotFoundException
    {
        public TenantNotFoundException(Guid Id) : base($"Tenant com Id {Id} não encontrado, ou você não tem permissão para acessá-lo.")
        {
        }
    }
}
