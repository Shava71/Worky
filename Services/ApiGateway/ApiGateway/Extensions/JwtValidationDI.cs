using System.Collections;
using ApiGateway.Service;
using ApiGateway.service.Interface;

namespace ApiGateway.DI;

public static class JwtValidationDI
{
    public static IServiceCollection AddJwtValidation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPublicRouteService, PublicRouteService>();
        
        return services;
    }
}