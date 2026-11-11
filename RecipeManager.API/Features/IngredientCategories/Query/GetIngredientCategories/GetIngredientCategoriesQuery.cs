using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.IngredientCategories.Query.GetIngredientCategories;

internal sealed record GetIngredientCategoriesProjection(Guid Id, string Name);

public sealed record GetIngredientCategoriesResponse(Guid Id, string Name, ItemActionDto Actions)
{
    internal static GetIngredientCategoriesResponse FromProjection(
        GetIngredientCategoriesProjection category,
        bool canEdit) =>
        new(category.Id, category.Name, new ItemActionDto(true, canEdit, canEdit));
}

public sealed record GetIngredientCategoriesCollectionResponse(
    CollectionActionDto Actions,
    IEnumerable<GetIngredientCategoriesResponse> Categories);

public sealed class GetIngredientCategoriesQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task<GetIngredientCategoriesCollectionResponse> Handle(
        CancellationToken cancellationToken)
    {
        List<GetIngredientCategoriesProjection> categories = await dbContext.IngredientCategories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new GetIngredientCategoriesProjection(category.Id.Value, category.Name))
            .ToListAsync(cancellationToken);

        bool canEdit = currentUser.HasRole(UserRoles.Admin);
        IEnumerable<GetIngredientCategoriesResponse> responses = categories
            .Select(category => GetIngredientCategoriesResponse.FromProjection(category, canEdit));
        return new GetIngredientCategoriesCollectionResponse(new CollectionActionDto(canEdit), responses);
    }
}
