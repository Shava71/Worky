using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WorkerService.BLL.Services.Http.Interfaces;
using WorkerService.DAL.Clients;

namespace WorkerService.BLL.Services.Http.Implementations;

public class AuthClient : IAuthClient
{
    
    private readonly ILogger<AuthClient> _logger;
    private readonly HttpClient _httpClient;

    public AuthClient(ILogger<AuthClient> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }
    
    public async Task<UserResponse?> GetUserByIdAsync(string userId, string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.GetAsync($"api/user/profile?userId={userId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"GetUserByIdAsync failed with status code {response.StatusCode}");
                return null;
            }
            return await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling AuthService for user {UserId}", userId);
            return null;
        }
    }
}