using FilterService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilterService.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<TypeOfActivity> TypeOfActivities { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var activities = modelBuilder.Entity<TypeOfActivity>();
        activities.Property(x => x.direction).IsRequired().HasColumnType("varchar(255)");
        activities.Property(x => x.type).IsRequired().HasColumnType("varchar(255)");
        
        activities.HasIndex(x => new { x.direction, x.type })
            .HasDatabaseName("idx_direction_type");

        base.OnModelCreating(modelBuilder);
    }
}