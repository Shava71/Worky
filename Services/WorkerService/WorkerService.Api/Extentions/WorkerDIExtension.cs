using WorkerService.BLL.Services.Interfaces;
using WorkerService.BLL.Services.Implementations;
using WorkerService.DAL.Data.DbConnection.Implementation;
using WorkerService.DAL.Data.DbConnection.Interface;
using WorkerService.DAL.Repositories.Implementations;
using WorkerService.DAL.Repositories.Interfaces;
using WorkerService.DAL.Repositories.Redis.Implementations;
using WorkerService.DAL.Repositories.Redis.Interfaces;

namespace WorkerService.Api.Extentions;

public static class WorkerDIExtension
{
    public static IServiceCollection AddWorkerServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, NpsqlDbConnectionFactory>();
        services.AddScoped<IResumeRepository, ResumeRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        //services.AddScoped<IFilterRedisRepository, FilterRedisRepository>();
        services.AddScoped<IWorkerService, BLL.Services.Implementations.WorkerService>();
        return services;
    }
}