using Worky.Repositories.Implementations;
using Worky.Repositories.Interfaces;
using Worky.Services;

namespace Worky.DI;

public static class ServiceExtention
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        services.AddScoped<IResumeRepository, ResumeRepository>();
        services.AddScoped<IVacancyRepository, VacancyRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IInfoRepository, InfoRepository>();
        return services;
    }
    
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICompnayService, CompanyService>();
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<IInfoService, InfoService>();
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }
    
}