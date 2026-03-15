namespace WorkerService.DAL.Repositories.Redis.Interfaces;

public interface IRedisRepository
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);

    Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiry = null);
    Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys);
}