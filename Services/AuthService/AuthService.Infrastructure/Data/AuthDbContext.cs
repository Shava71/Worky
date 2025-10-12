using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace AuthService.Infrastructure.Data;

public class AuthDbContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<UserRole> UserRole { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Role>().HasKey(r => r.Id);
        
        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        // add foreign key
        modelBuilder.Entity<UserRole>() 
            .HasOne(ur => ur.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(ur => ur.RoleId);
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(ur => ur.RoleId);
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(r => r.Roles)
            .HasForeignKey(ur => ur.UserId);
    }
}