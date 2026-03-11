using AuthService.Application.Services;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Kafka;
using AuthService.Infrastructure.Repository.Implementation;
using AuthService.Infrastructure.Repository.Interface;
using AuthService.Infrastructure.Worker;
using Minio;

namespace AuthService.Api.Extentions;

public static class AuthDIExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddSingleton<KafkaProducerFactory>();
        services.AddScoped<IOutboxPublisher, KafkaOutboxPublisher>();
        services.AddHostedService<OutboxPublisWorker>();
        
        services.AddSingleton<IMinioClient>(sp =>
        {
            var config = configuration.GetSection("Minio");

            return new MinioClient()
                .WithEndpoint(config["Endpoint"])
                .WithCredentials(config["AccessKey"], config["SecretKey"])
                .WithSSL(false)
                .Build();
        });
        services.AddSingleton<MinioBucketInitializer>();
        
        return services;
    } 
}