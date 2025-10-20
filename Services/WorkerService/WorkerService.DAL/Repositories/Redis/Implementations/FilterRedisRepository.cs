using Microsoft.Extensions.Caching.Distributed;
using WorkerService.DAL.Repositories.Redis.Interfaces;

namespace WorkerService.DAL.Repositories.Redis.Implementations;

public class FilterRedisRepository : IFilterRedisRepository
{
    private readonly IDistributedCache _cache;
    private string filterKey = "filter_toker:";

    public FilterRedisRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetTokenAsync(string filterId, string token)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        await _cache.SetStringAsync($"{filterKey}{filterId}", token, options);
    }

    public async Task<string?> GetTokenAsync(string filterId)
    {
        return await _cache.GetStringAsync($"{filterKey}{filterId}");
    }

    public async Task RemoveTokenAsync(string filterId)
    {
        await _cache.RemoveAsync($"{filterKey}{filterId}");
    }
}