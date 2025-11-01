using FeedbackService.BLL.Services.Interfaces;
using FeedbackService.DAL.Repositories.Implementations;
using FeedbackService.DAL.Repositories.Interfaces;

namespace FeedbackService.Api.Extensions;

public static class FeedbackDIExtensions
{
    public static IServiceCollection AddFeedbackDI(this IServiceCollection services)
    {
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IFeedbackService, BLL.Services.Implementations.FeedbackService>();

        return services;
    }
}