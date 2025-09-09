using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateMultTenaBackend.Domain.Entities;

namespace TemplateMultTenaBackend.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("d5bc83ab-df19-4327-9367-9ce32d041c14"),
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                },
                new Role
                {
                    Id = Guid.Parse("07e4613a-e7ac-45f0-adab-4f12ed7f7da4"),
                    Name = "Member",
                    NormalizedName = "MEMBER"
                }
            );
        }
    }
}
