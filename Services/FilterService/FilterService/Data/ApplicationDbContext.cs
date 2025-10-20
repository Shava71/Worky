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
        activities.HasKey(x => x.id);
        activities.Property(x => x.direction).IsRequired();
        activities.Property(x => x.type).IsRequired();
        
    }
}