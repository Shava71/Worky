// <copyright file="HttpClientRegistrationExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using CompanyService.BLL.Services.Http.Implementations;
using CompanyService.BLL.Services.Http.Interfaces;
using Polly;
using Polly.Extensions.Http;

namespace CompanyService.Api.Extentions;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddExternalHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAuthClient, AuthClient>(client =>
        {
            string? baseUrl = configuration["authservice:Url"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("authservice:Url is not configured.");
            }

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        services.AddHttpClient<IFilterClient, FilterClient>(client =>
        {
            string? baseUrl = configuration["filterservice:Url"];

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("authservice:Url is not configured.");
            }

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
}