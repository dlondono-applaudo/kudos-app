using KudosApp.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Department).HasMaxLength(100).IsRequired();
            entity.Property(u => u.AvatarUrl).HasMaxLength(500);
            entity.Property(u => u.CreatedAt).IsRequired();
        });
    }
}
