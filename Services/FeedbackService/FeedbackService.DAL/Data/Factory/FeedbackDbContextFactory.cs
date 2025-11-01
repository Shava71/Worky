using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FeedbackService.DAL.Data.Factory;

public class FeedbackDbContextFactory : IDesignTimeDbContextFactory<FeedbackDbContext>
{
    public FeedbackDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot confirutation = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        string connectionString = confirutation.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<FeedbackDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new FeedbackDbContext(optionsBuilder.Options);
    }
}