using System.Text.Json;
using RecipeManager.Api.Application.Abstractions;
using StackExchange.Redis;

namespace RecipeManager.Api.Application.Services;

public class RedisService(IConnectionMultiplexer connection) : IRedisService
{
    private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private readonly IDatabase _database = connection.GetDatabase();

    public async Task<T?> GetValue<T>(string key)
           where T : class
    {
        RedisValue result = await _database.StringGetAsync(key);
        return result.IsNullOrEmpty ? null : JsonSerializer.Deserialize<T>((string)result!, _options);

    }

    public async Task<bool> RemoveValue(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task SetValue<T>(string key, T value, TimeSpan? expiry = null)
           where T : class
    {
        Expiration expiration = expiry.HasValue ? new Expiration(expiry.Value) : default;
        await _database.StringSetAsync(key, JsonSerializer.Serialize(value, _options), expiration);
    }
}
