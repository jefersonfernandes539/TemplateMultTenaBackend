using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TemplateMultTenaBackend.Domain.Entities;
using TemplateMultTenaBackend.Infrastructure.Persistence.Configurations;

namespace TemplateMultTenaBackend.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.Entity<TenantUser>()
                .HasOne(tu => tu.CreatedBy)
                .WithMany()
                .HasForeignKey(nameof(TenantUser.CreatedById))
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TenantUser>()
                .HasOne(tu => tu.UpdatedBy)
                .WithMany()
                .HasForeignKey(nameof(TenantUser.UpdatedById))
                .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantUser> TenantUsers { get; set; }
        public DbSet<UserRefreshToken> UserRefreshToken { get; set; }
        public DbSet<MagicLink> MagicLinks { get; set; }
    }
}
