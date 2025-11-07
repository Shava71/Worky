using System.Net.Http.Json;
using CompanyService.BLL.Services.Http.Interfaces;
using CompanyService.DAL.HttpClients.Clients;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace CompanyService.BLL.Services.Http.Implementations;

public class FilterClient : IFilterClient
{
    private readonly ILogger<FilterClient> _logger;
    private readonly HttpClient _httpClient;
    
    public FilterClient(HttpClient client, ILogger<FilterClient> logger)
    {
        _httpClient = client;
        _logger = logger;
    }
    
    public async Task<List<TypeOfActivityResponse?>> GetFiltersByIdAsync(List<int> filterIds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string url = "api/Filter/GetFilters";
            if (filterIds.Count != 0)
            {
                IEnumerable<KeyValuePair<string,string>> queryParams = filterIds.Select(id => new KeyValuePair<string, string>("filterIds", id.ToString()));
                url = QueryHelpers.AddQueryString(url, queryParams);
            }
            
            HttpResponseMessage response  = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"GetFilters failed with status code {response.StatusCode}");
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<TypeOfActivityResponse>>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling FilterService {ex.Message}");
            return null;
        }
    }
}