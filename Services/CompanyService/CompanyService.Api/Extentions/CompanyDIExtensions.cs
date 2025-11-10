

using CompanyService.BLL.Services.Implementations;
using CompanyService.BLL.Services.Interfaces;
using CompanyService.DAL.Data.DbConnection.Implementation;
using CompanyService.DAL.Data.DbConnection.Interface;
using CompanyService.DAL.Repositories.Interfaces;
using CompanyService.DAL.Repositories.Redis.Implementations;
using CompanyService.DAL.Repositories.Redis.Interfaces;
using Worky.Repositories.Implementations;

namespace CompanyService.Api.Extentions;

public static class CompanyDIExtensions
{
    public static IServiceCollection AddCompanyService(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, NpsqlDbConnectionFactory>();
        services.AddScoped<IRedisRepository, RedisRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IVacancyRepository, VacancyRepository>();
        services.AddScoped<IFilterCacheService, FilterCacheService>();
        services.AddScoped<ICompnayService, BLL.Services.Implementations.CompanyService>();
        
        return services;
    }
}