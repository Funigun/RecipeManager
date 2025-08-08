using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Ingredients;

public static class GetIngredientById
{
    public record struct Request(Guid Value) : IRequestId<Request>
    {
    }

    public sealed record IngredientRecipeDto(Guid Id, string Title);

    public sealed record IngredientCategoryDto(Guid Id, string Name);

    public sealed record IngredientDto(Guid Id, string Name, IEnumerable<IngredientRecipeDto> Recipes, IEnumerable<IngredientCategoryDto> Categories);

    public sealed record Response(Guid Id, string Name, IEnumerable<HateoasResponse<IngredientRecipeDto>> Recipes, IEnumerable<IngredientCategoryDto> Categories);

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedGet<Response>("/{ingredientId}", Handler)
                     .WithName("GetIngredientById")
                     .WithDescription("Gets an ingredient by its ID");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(Request ingredientId, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IngredientId id = new(ingredientId.Value);

        Ingredient? ingredient = await GetIngredient(dbContext, id, cancellationToken)
                              ?? throw new EntityNotFoundException<Ingredient, IngredientId>(id);

        IEnumerable<IngredientRecipeDto> recipes = await GetIngredientRecipes(dbContext, ingredient, cancellationToken);
        IEnumerable<IngredientCategoryDto> categories = await GetIngredientCategories(dbContext, ingredient, cancellationToken);

        IngredientDto ingredientDto = MapToIngredientDto(ingredient, recipes, categories);

        HateoasResponse<Response> hateoasResponse = MapToHateoasResponse(ingredientDto, hateoasBuilderFactory);

        return TypedResults.Ok(hateoasResponse);
    }

    private static async Task<Ingredient?> GetIngredient(IAppDbContext dbContext, IngredientId ingredientId, CancellationToken cancellationToken)
    {
        return await dbContext.Ingredients.AsNoTracking()
                                          .Include(i => i.Categories)
                                          .Include(i => i.Recipes)
                                          .FirstOrDefaultAsync(i => i.Id == ingredientId, cancellationToken);
    }

    private static async Task<IEnumerable<IngredientRecipeDto>> GetIngredientRecipes(IAppDbContext dbContext, Ingredient ingredient, CancellationToken cancellationToken)
    {
        return ingredient.Recipes.Any()
             ? await dbContext.Recipes.AsNoTracking()
                                      .Where(r => ingredient.Recipes.Contains(r.Id))
                                      .Select(r => new IngredientRecipeDto(r.Id, r.Title))
                                      .ToListAsync(cancellationToken)
             : [];
    }

    private static async Task<IEnumerable<IngredientCategoryDto>> GetIngredientCategories(IAppDbContext dbContext, Ingredient ingredient, CancellationToken cancellationToken)
    {
        return ingredient.Categories.Any()
             ? await dbContext.IngredientCategories.AsNoTracking()
                                      .Where(c => ingredient.Categories.Contains(c.Id))
                                      .Select(r => new IngredientCategoryDto(r.Id, r.Name))
                                      .ToListAsync(cancellationToken)
             : [];
    }

    private static IngredientDto MapToIngredientDto(Ingredient ingredient, IEnumerable<IngredientRecipeDto> recipes, IEnumerable<IngredientCategoryDto> categories)
    {
        return new IngredientDto(ingredient.Id.Value, ingredient.Name, recipes, categories);
    }

    private static HateoasResponse<Response> MapToHateoasResponse(IngredientDto ingredientDto, IHateoasBuilderFactory hateoasBuilderFactory)
    {
        HateoasCollectionResponseBuilder<IngredientRecipeDto> recipesBuilder = hateoasBuilderFactory.ForCollection(ingredientDto.Recipes);

        recipesBuilder.WithCollectionLink()
                      .WithGet(LinkOptions.Create("GetRecipeById", HateoasRelConstants.Self, true), recipe => new { recipeId = recipe.Id });

        Response response = new(ingredientDto.Id, ingredientDto.Name, recipesBuilder.Build().Items, ingredientDto.Categories);

        HateoasResponseBuilder<Response> responseBuilder = hateoasBuilderFactory.ForItem(response);

        responseBuilder.AddGet(LinkOptions.Create("GetIngredientById", HateoasRelConstants.Self, true), new { ingredientId = ingredientDto.Id })
                       .AddPut(LinkOptions.Create("UpdateIngredient", HateoasRelConstants.Create, true), new { ingredientId = ingredientDto.Id })
                       .AddDelete(LinkOptions.Create("UpdateIngredient", HateoasRelConstants.Delete, true), new { ingredientId = ingredientDto.Id });

        return responseBuilder.Build();
    }
}
