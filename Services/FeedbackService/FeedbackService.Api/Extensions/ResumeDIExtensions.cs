using FeedbackService.DAL.Repositories.Implementations;
using FeedbackService.DAL.Repositories.Interfaces;

namespace FeedbackService.Api.Extensions;

public static class ResumeDIExtensions
{
    public static IServiceCollection AddResumeDI(this IServiceCollection services)
    {
        services.AddScoped<IResumeRepository, ResumeRepository>();

        return services;
    }
}