namespace TemplateMultTenaBackend.Domain.Exceptions.NotFound
{
    public sealed class TenantUserNotFoundException : NotFoundException
    {
        public TenantUserNotFoundException() : base("Tenant/Usuário não encontrado ou você não tem permissão para ver este Tenant.")
        {
        }

        public TenantUserNotFoundException(Guid Id) : base($"Nenhum usuário com {Id} foi encontrado no seu Tenant atual.")
        {
        }
    }
}
