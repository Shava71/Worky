using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using WorkerService.DAL.Entities;

namespace WorkerService.DAL.Data;

public class WorkerDbContext : DbContext
{
    public DbSet<Worker> Worker { get; set; }
    public DbSet<Resume> Resume { get; set; }
    public DbSet<Resume_filter> Resume_filter { get; set; }
    public DbSet<Education> Education { get; set; }
    

    
    public WorkerDbContext(DbContextOptions<WorkerDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure MassTransit OutBox Entities
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        
        modelBuilder.ApplyConfiguration(new WorkerConfiguration());
        modelBuilder.ApplyConfiguration(new ResumeConfiguration());
        modelBuilder.ApplyConfiguration(new ResumeFilterConfiguration());
        modelBuilder.ApplyConfiguration(new EducationConfiguration());
    }
}