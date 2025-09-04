namespace RecipeManager.Api.Application.Abstractions;

public interface IRedisService
{
    Task SetValue<T>(string key, T value, TimeSpan? expiry = null)
         where T : class;

    Task<T?> GetValue<T>(string key)
             where T : class;

    Task<bool> RemoveValue(string key);
}
