using System.Text.Json;
using CompanyService.DAL.Repositories.Redis.Interfaces;

namespace CompanyService.Tests.Integration;

public class InMemoryRedisRepository : IRedisRepository
{
    private class Entry
    {
        public string Json { get; set; } = default!;
        public DateTime? ExpireAt { get; set; }
    }

    private readonly Dictionary<string, Entry> _storage = new();

    private void CleanupExpired(string key)
    {
        if (_storage.TryGetValue(key, out var entry))
        {
            if (entry.ExpireAt.HasValue && entry.ExpireAt.Value <= DateTime.UtcNow)
            {
                _storage.Remove(key);
            }
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        _storage[key] = new Entry
        {
            Json = JsonSerializer.Serialize(value),
            ExpireAt = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : null
        };

        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        CleanupExpired(key);

        if (!_storage.TryGetValue(key, out var entry))
            return Task.FromResult<T?>(default);

        var value = JsonSerializer.Deserialize<T>(entry.Json);
        return Task.FromResult(value);
    }

    public Task RemoveAsync(string key)
    {
        _storage.Remove(key);
        return Task.CompletedTask;
    }

    public Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiry = null)
    {
        foreach (var kv in values)
        {
            _storage[kv.Key] = new Entry
            {
                Json = JsonSerializer.Serialize(kv.Value),
                ExpireAt = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : null
            };
        }

        return Task.CompletedTask;
    }

    public Task<Dictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys)
    {
        var result = new Dictionary<string, T?>();

        foreach (var key in keys)
        {
            CleanupExpired(key);

            if (_storage.TryGetValue(key, out var entry))
            {
                result[key] = JsonSerializer.Deserialize<T>(entry.Json);
            }
            else
            {
                result[key] = default;
            }
        }

        return Task.FromResult(result);
    }
}