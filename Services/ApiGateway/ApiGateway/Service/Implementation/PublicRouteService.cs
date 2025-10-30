using ApiGateway.service.Interface;

namespace ApiGateway.Service;

public class PublicRouteService : IPublicRouteService
{
    private readonly HashSet<string> _publicRoutes;

    public PublicRouteService(IConfiguration configuration)
    {
        _publicRoutes = configuration.GetSection("PublicRoutes").Get<string[]>()?.ToHashSet() ??
                        new HashSet<string>();
    }
    

    public bool IsPublicRoute(string route)
    {
        return _publicRoutes.Any(p => p.StartsWith(route, StringComparison.OrdinalIgnoreCase));
    }
}