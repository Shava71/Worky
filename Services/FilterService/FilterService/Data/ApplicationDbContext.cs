using FilterService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilterService.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<TypeOfActivity> TypeOfActivities { get; set; }
    public DbSet<Education> Educations { get; set; }
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
        
        EntityTypeBuilder<Education> educations = modelBuilder.Entity<Education>();
        educations.HasKey(e => e.id);
        educations.HasData(new Education[]
        {
            new Education() { id = 1, name = "Начальное общее образование" },
            new Education() { id = 2, name = "Основное общее образование" },
            new Education() { id = 3, name = "Среднее общее образование" },
            new Education() { id = 4, name = "Среднее профессиональное образование" },
            new Education() { id = 5, name = "Бакалавриат" },
            new Education() { id = 6, name = "Специалитет" },
            new Education() { id = 7, name = "Магистратура" },
            new Education() { id = 8, name = "Аспирантура" },
            new Education() { id = 9, name = "Ординатура" },
        });

        base.OnModelCreating(modelBuilder);
    }
}