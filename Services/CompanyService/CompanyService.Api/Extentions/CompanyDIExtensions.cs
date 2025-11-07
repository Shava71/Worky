

using CompanyService.DAL.Data.DbConnection.Implementation;
using CompanyService.DAL.Data.DbConnection.Interface;
using CompanyService.DAL.Repositories.Interfaces;
using Worky.Repositories.Implementations;

namespace CompanyService.Api.Extentions;

public static class CompanyDIExtensions
{
    public static IServiceCollection AddCompanyService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, NpsqlDbConnectionFactory>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        
        return services;
    }
}