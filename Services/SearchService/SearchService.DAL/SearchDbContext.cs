using Microsoft.EntityFrameworkCore;
using SearchService.DAL.Entities;

namespace SearchService.DAL;

public class SearchDbContext : DbContext
{
    
    public SearchDbContext(DbContextOptions<SearchDbContext> options)
        : base(options) { }

    public DbSet<SearchSession> SearchSessions {get;set;}
    public DbSet<SearchImpression> SearchImpressions {get;set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<SearchImpression>()
            .HasOne(i => i.Session)
            .WithMany(s => s.Impressions)
            .HasForeignKey(i => i.SessionId);
    }
}