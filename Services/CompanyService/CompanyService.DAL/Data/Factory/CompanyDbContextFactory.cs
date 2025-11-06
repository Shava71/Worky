using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using CompanyService.DAL.Data;

namespace WorkerService.DAL.Data.Factory;

public class CompanyDbContextFactory : IDesignTimeDbContextFactory<CompanyDbContext>
{
    public CompanyDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot confirutation = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        string connectionString = confirutation.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<CompanyDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new CompanyDbContext(optionsBuilder.Options);
    }
}