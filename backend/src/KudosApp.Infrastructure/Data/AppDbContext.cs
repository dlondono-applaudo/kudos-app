using KudosApp.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KudosApp.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Kudos> Kudos => Set<Kudos>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Department).HasMaxLength(100).IsRequired();
            entity.Property(u => u.AvatarUrl).HasMaxLength(500);
            entity.Property(u => u.CreatedAt).IsRequired();
        });

        // Category
        builder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(50).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(200).IsRequired();
            entity.Property(c => c.PointValue).IsRequired();

            // Seed default categories
            entity.HasData(
                new { Id = 1, Name = "Teamwork", Description = "Great collaboration and team spirit", PointValue = 10 },
                new { Id = 2, Name = "Innovation", Description = "Creative thinking and new ideas", PointValue = 15 },
                new { Id = 3, Name = "Leadership", Description = "Leading by example and mentoring others", PointValue = 20 },
                new { Id = 4, Name = "Problem Solving", Description = "Finding solutions to tough challenges", PointValue = 15 },
                new { Id = 5, Name = "Going Extra Mile", Description = "Exceeding expectations consistently", PointValue = 25 }
            );
        });

        // Kudos
        builder.Entity<Kudos>(entity =>
        {
            entity.HasKey(k => k.Id);
            entity.Property(k => k.Message).HasMaxLength(500).IsRequired();
            entity.Property(k => k.Points).IsRequired();
            entity.Property(k => k.CreatedAt).IsRequired();

            entity.HasOne(k => k.Sender)
                .WithMany()
                .HasForeignKey(k => k.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(k => k.Receiver)
                .WithMany()
                .HasForeignKey(k => k.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(k => k.Category)
                .WithMany()
                .HasForeignKey(k => k.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(k => k.CreatedAt);
            entity.HasIndex(k => k.ReceiverId);
        });

        // Badge
        builder.Entity<Badge>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).HasMaxLength(50).IsRequired();
            entity.Property(b => b.Description).HasMaxLength(200).IsRequired();
            entity.Property(b => b.Icon).HasMaxLength(50).IsRequired();
            entity.Property(b => b.RequiredPoints).IsRequired();

            entity.HasData(
                new { Id = 1, Name = "First Kudos", Description = "Received your first kudos", Icon = "star", RequiredPoints = 10 },
                new { Id = 2, Name = "Team Player", Description = "Received 50 points", Icon = "people", RequiredPoints = 50 },
                new { Id = 3, Name = "Superstar", Description = "Received 100 points", Icon = "trophy", RequiredPoints = 100 },
                new { Id = 4, Name = "Legend", Description = "Received 250 points", Icon = "crown", RequiredPoints = 250 }
            );
        });

        // UserBadge
        builder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(ub => ub.Id);
            entity.HasOne(ub => ub.User).WithMany().HasForeignKey(ub => ub.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ub => ub.Badge).WithMany().HasForeignKey(ub => ub.BadgeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(ub => new { ub.UserId, ub.BadgeId }).IsUnique();
        });

        // Notification
        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Message).HasMaxLength(300).IsRequired();
            entity.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(n => new { n.UserId, n.IsRead });
        });

        // AuditLog
        builder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Action).HasMaxLength(50).IsRequired();
            entity.Property(a => a.EntityType).HasMaxLength(50).IsRequired();
            entity.Property(a => a.EntityId).HasMaxLength(50).IsRequired();
            entity.HasIndex(a => a.CreatedAt);
        });
    }
}
