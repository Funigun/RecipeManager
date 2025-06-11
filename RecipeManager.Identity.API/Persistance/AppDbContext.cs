using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Identity.API.Domain;

namespace RecipeManager.Identity.API.Persistance;

public sealed class AppDbContext : IdentityDbContext<User, Role, int>
{
    public DbSet<Permission> Permissions { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);  

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");

            b.HasMany<Permission>()
             .WithMany()
             .UsingEntity("UserPermissions");
        });

        builder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.HasMany<Permission>()
             .WithMany()
             .UsingEntity("RolePermissions");
        });
    }
}
