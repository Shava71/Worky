using SearchService.BLL.Services.Implementations;
using SearchService.BLL.Services.Interfaces;

namespace SearchService.Api.Extensions;

public static class AddElasticServicesExtension
{
    public static IServiceCollection AddElasticServices(this IServiceCollection services)
    {
        services.AddScoped<IVacancySearchService, VacancySearchService>();
        services.AddScoped<IResumeSearchService, ResumeSearchService>();
        
        return services;
    }
}