using AuthService.Application.Services;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Repository.Implementation;
using AuthService.Infrastructure.Repository.Interface;

namespace AuthService.Api.Extentions;

public static class AuthDI
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        
        return services;
    } 
}