using CompanyService.DAL.Entities;
using CompanyService.DAL.Entities.TypeConfiguration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using WorkerService.DAL.Entities;

namespace CompanyService.DAL.Data;

public class CompanyDbContext : DbContext
{
    public DbSet<Company> company { get; set; }
    public DbSet<Deal> deal { get; set; }
    public DbSet<Education> education { get; set; }
    public DbSet<Tarrif?> tariff { get; set; }
    public DbSet<Vacancy> vacancy { get; set; }
    public DbSet<Vacancy_filter?> vacancy_filter { get; set; }
    
    
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options){}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure MassTransit OutBox Entities
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.ApplyConfiguration(new EducationConfiguration());
        modelBuilder.ApplyConfiguration(new VacancyConfiguration());
        modelBuilder.ApplyConfiguration(new VacancyFilterConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new TarrifConfiguration());
        modelBuilder.ApplyConfiguration(new DealConfiguration());
    }
}