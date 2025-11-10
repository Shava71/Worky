using FeedbackService.DAL.Entities;
using FeedbackService.DAL.Repositories.Implementations;
using FeedbackService.DAL.Repositories.Interfaces;

namespace FeedbackService.Api.Extensions;

public static class VacancyDiExtensions
{
    public static IServiceCollection AddVacancyDI(this IServiceCollection services)
    {
        services.AddScoped<IVacancyRepository, VacancyRepository>();
        
        return services;
    }
}