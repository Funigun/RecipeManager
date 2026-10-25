using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.IngredientCategories.Query.GetIngredientCategories;

internal sealed record GetIngredientCategoriesProjection(Guid Id, string Name);

public sealed record GetIngredientCategoriesResponse(Guid Id, string Name)
{
    internal static GetIngredientCategoriesResponse FromProjection(GetIngredientCategoriesProjection category) =>
        new(category.Id, category.Name);
}

public sealed class GetIngredientCategoriesQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IHateoasBuilderFactory hateoasBuilderFactory)
{
    public async Task<HateoasCollectionResponse<GetIngredientCategoriesResponse>> Handle(
        CancellationToken cancellationToken)
    {
        List<GetIngredientCategoriesProjection> categories = await dbContext.IngredientCategories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new GetIngredientCategoriesProjection(category.Id.Value, category.Name))
            .ToListAsync(cancellationToken);

        IEnumerable<GetIngredientCategoriesResponse> responses = categories
            .Select(GetIngredientCategoriesResponse.FromProjection);
        bool canEdit = currentUser.HasRole(UserRoles.Admin);
        HateoasCollectionResponseBuilder<GetIngredientCategoriesResponse> collectionBuilder =
            hateoasBuilderFactory.ForCollection(responses);

        if (canEdit)
        {
            collectionBuilder
                .WithCollectionLink()
                    .WithDelete(
                        LinkOptions.Create("DeleteIngredientCategory", HateoasRelConstants.Delete, canEdit),
                        category => new { categoryId = category.Id })
                .AddPost(LinkOptions.Create("CreateIngredientCategory", HateoasRelConstants.Create, canEdit), null);
        }

        return collectionBuilder.Build();
    }
}
