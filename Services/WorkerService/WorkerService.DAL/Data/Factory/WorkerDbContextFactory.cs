using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WorkerService.DAL.Data.Factory;

public class WorkerDbContextFactory : IDesignTimeDbContextFactory<WorkerDbContext>
{
    public WorkerDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot confirutation = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        string connectionString = confirutation.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<WorkerDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new WorkerDbContext(optionsBuilder.Options);
    }
}