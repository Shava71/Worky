using FeedbackService.DAL.Data.TypeConfiguration;
using FeedbackService.DAL.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.DAL.Data;

public class FeedbackDbContext : DbContext
{
    public DbSet<Feedback?> feedback { get; set; }
    public DbSet<Resume> resume { get; set; }
    public DbSet<Vacancy> vacancy { get; set; }
    
    public FeedbackDbContext(DbContextOptions<FeedbackDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure MassTransit OutBox Entities
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
    }
}