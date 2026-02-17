using System.Text.Json;
using CompanyService.DAL.Repositories.Redis.Interfaces;
using StackExchange.Redis;

namespace CompanyService.DAL.Repositories.Redis.Implementations;

public class  RedisRepository : IRedisRepository
{
    private readonly IDatabase _database;

    public RedisRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        string json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        RedisValue value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiry = null)
    {
        KeyValuePair<RedisKey, RedisValue>[] entries = values.Select(kv => new KeyValuePair<RedisKey, RedisValue>(
            kv.Key,
            JsonSerializer.Serialize(kv.Value)
        )).ToArray();

        await _database.StringSetAsync(entries);

        if (expiry.HasValue)
        {
            foreach (var key in values.Keys)
                await _database.KeyExpireAsync(key, expiry);
        }
    }

    public async Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys)
    {
        RedisKey[] redisKeys = keys.Select(k => (RedisKey)k).ToArray();
        RedisValue[] values = await _database.StringGetAsync(redisKeys);

        Dictionary<string, T?> result = new Dictionary<string, T?>();
        int i = 0;
        foreach (string key in keys)
        {
            if (!values[i].IsNullOrEmpty)
                result[key] = JsonSerializer.Deserialize<T>(values[i]!);
            else
                result[key] = default;
            i++;
        }

        return result;
    }
}