using SearchService.DAL.Repositories.Implementations;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.Api.Extensions;

public static class AddElasticRepositoriesExtension
{
    public static IServiceCollection AddElasticRepositories(this IServiceCollection services)
    {
        services.AddScoped<IResumeElasticRepository, ResumeElasticRepository>();
        services.AddScoped<IVacancyElasticRepository, VacancyElasticRepository>();
        
        return services;
    }  
}