using AuthService.Application.Services;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Kafka;
using AuthService.Infrastructure.Repository.Implementation;
using AuthService.Infrastructure.Repository.Interface;
using AuthService.Infrastructure.Worker;

namespace AuthService.Api.Extentions;

public static class AuthDI
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddSingleton<KafkaProducerFactory>();
        services.AddScoped<IOutboxPublisher, KafkaOutboxPublisher>();
        services.AddHostedService<OutboxPublisWorker>();
        
        return services;
    } 
}