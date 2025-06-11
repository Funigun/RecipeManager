using Microsoft.AspNetCore.Http;

namespace RecipeManager.Api.Shared.Contracts.Authorization;

public interface IAuthorizationPolicy<TRequest>
{
    Task<bool> IsAuthorized(TRequest request);
}
