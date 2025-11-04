using Microsoft.Extensions.Logging;
using WorkerService.BLL.Services.Http.Interfaces;
using WorkerService.BLL.Services.Interfaces;
using WorkerService.DAL.HttpClients.Clients;
using WorkerService.DAL.Repositories.Interfaces;
using WorkerService.DAL.Repositories.Redis.Interfaces;

namespace WorkerService.BLL.Services.Implementations;

public class FilterCacheService : IFilterCacheService
{
    private readonly ILogger<FilterCacheService> _logger;
    private readonly IRedisRepository _redisRepository;
    private readonly IFilterClient _filterClient;
    private const string FilterKeyPrefix = "filter:";

    public FilterCacheService(ILogger<FilterCacheService> logger,
        IRedisRepository redisRepository,
        IFilterClient filterClient)
    {
        _logger = logger;
        _redisRepository = redisRepository;
        _filterClient = filterClient;
    }
    
    public async Task<List<TypeOfActivityResponse>?> GetFiltersByIdsAsync(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return null;
        }
        
        List<string> keys = ids.Select(id => $"{FilterKeyPrefix}{id}").ToList();

        Dictionary<string, TypeOfActivityResponse> cachedFilters = await _redisRepository.GetManyAsync<TypeOfActivityResponse>(keys);
        
        Dictionary<string, TypeOfActivityResponse> found = cachedFilters
            .Where(kv => kv.Value != null).
            ToDictionary(kv => kv.Key, kv => kv.Value);

        List<int> missing = ids
            .Where(id => !found.ContainsKey($"{FilterKeyPrefix}{id}"))
            .ToList();

        if (missing.Count > 0) // недостающее вытянем из FilterService
        {
            List<TypeOfActivityResponse> acvtivityFilters = await _filterClient.GetFiltersByIdAsync(missing);

            if (acvtivityFilters != null && acvtivityFilters.Count > 0)
            {
                Dictionary<string, TypeOfActivityResponse> dict = acvtivityFilters.ToDictionary(
                    f => $"{FilterKeyPrefix}{f.id}",
                    f => f);

                await _redisRepository.SetManyAsync<TypeOfActivityResponse>(dict, TimeSpan.FromHours(1));

                foreach (KeyValuePair<string, TypeOfActivityResponse> kv in dict)
                {
                    found.Add($"{FilterKeyPrefix}{kv.Key}", kv.Value);
                }
            }
        }
        
        return found.Values.ToList();
    }
}