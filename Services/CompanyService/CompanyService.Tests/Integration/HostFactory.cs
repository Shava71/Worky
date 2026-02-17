using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace CompanyService.Tests.Integration;

public static class HostFactory
{
    public static IHost CreateHost()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        var app = builder.Build();

        app.MapGet("/api/Filter/GetFilters", async (HttpRequest request) =>
        {
            var ids = request.Query["filterIds"]
                .Select(int.Parse)
                .ToList();

            var result = TestData.Activities
                .Where(a => ids.Contains(a.id))
                .ToList();

            return Results.Json(result);
        });

        app.Start();

        return app;
    }
}