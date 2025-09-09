namespace TemplateMultTenaBackend.Domain.Entities.Enums
{
    public static class Roles
    {
        public static Guid Admininstrator = Guid.Parse("d5bc83ab-df19-4327-9367-9ce32d041c14");
        public static Guid Member = Guid.Parse("07e4613a-e7ac-45f0-adab-4f12ed7f7da4");
        public static Guid[] AnyRole = { Admininstrator, Member };
    }
}
