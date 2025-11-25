using SearchService.BLL.Services.Implementations;
using SearchService.BLL.Services.Interfaces;
using SearchService.ML;

namespace SearchService.Api.Extensions;

public static class AddElasticServicesExtension
{
    public static IServiceCollection AddElasticServices(this IServiceCollection services)
    {
        services.AddScoped<IVacancySearchService, VacancySearchService>();
        services.AddScoped<IResumeSearchService, ResumeSearchService>();

        services.AddScoped<SbertEmbeddingService>();
        
        return services;
    }
}