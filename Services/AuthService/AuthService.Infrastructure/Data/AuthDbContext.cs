using AuthService.Domain.Entities;
using AuthService.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;


namespace AuthService.Infrastructure.Data;

public class AuthDbContext : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    
    public DbSet<OutboxMessage> OutboxMessage { get; set; }

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
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(r => r.Roles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // ddl
        // ---------- USER ----------
        var user = modelBuilder.Entity<User>();
        user.ToTable("users");
        
        user.Property(u => u.Id)
            .IsRequired();

        user.Property(u => u.UserName)
            .HasMaxLength(256)
            .IsRequired();

        user.Property(u => u.NormalizedUserName)
            .HasMaxLength(256)
            .IsRequired();

        user.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        user.Property(u => u.NormalizedEmail)
            .HasMaxLength(256)
            .IsRequired();

        user.Property(u => u.PasswordHash)
            .IsRequired();

        user.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        user.Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        user.HasIndex(u => u.Email).IsUnique();
        user.HasIndex(u => u.UserName).IsUnique();
        user.HasIndex(u => u.NormalizedEmail).IsUnique();
        user.HasIndex(u => u.NormalizedUserName).IsUnique();
        

        // ---------- ROLE ----------
        var role = modelBuilder.Entity<Role>();

        role.ToTable("roles");

        role.HasKey(r => r.Id);

        role.Property(r => r.Id)
            .IsRequired();

        role.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        // ---------- OUTBOX ----------
        modelBuilder.Entity<OutboxMessage>(eb =>
        {
            eb.HasKey(o => o.Id);
            eb.Property(o => o.Type).IsRequired();
            eb.Property(o => o.Payload).IsRequired();
            eb.Property(o => o.Sent).HasDefaultValue(false);
        });
    }
}