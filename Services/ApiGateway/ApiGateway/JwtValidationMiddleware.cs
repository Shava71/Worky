using ApiGateway.service.Interface;

namespace ApiGateway.Middleware;

using Service;

public class JwtValidationMiddleware
{
    
    private readonly RequestDelegate _next;
    private readonly IPublicRouteService _publicRouteService;

    public JwtValidationMiddleware(IPublicRouteService publicRouteService, RequestDelegate next)
    {
        _publicRouteService = publicRouteService;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? path = context.Request.Path.Value?.ToLower();

        if (_publicRouteService.IsPublicRoute(path))
        {
            await _next.Invoke(context);
            return;
        }

        if (!context.User.Identity!.IsAuthenticated)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next.Invoke(context);
    }
 
}