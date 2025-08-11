using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Persistance.Extensions;

public static class QuerableExtensions
{
    public static IQueryable<TResponse> SetPage<TFilter, TResponse>(this IQueryable<TResponse> query, TFilter model)
            where TFilter : PagedParameters
    {
        return query.Skip((model.Page - 1) * model.PageSize)
                    .Take(model.PageSize);
    }
}
