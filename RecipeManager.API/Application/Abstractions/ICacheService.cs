namespace RecipeManager.Api.Application.Abstractions;

public interface ICacheService
{
    Task SetValue<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
         where T : class;

    Task<T?> GetValue<T>(string key, CancellationToken cancellationToken = default)
             where T : class;

    Task<T?> GetOrCreateAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    Task RemoveValue(string key, CancellationToken cancellationToken = default);
}
