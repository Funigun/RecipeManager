using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Ingredients;

public static class GetIngredientById
{
    public record struct Request(Guid Id) : IRequestId<Request>
    {
    }

    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);

    public sealed record IngredientRecipeDto(Guid Id, string Title);

    public sealed record IngredientCategoryDto(Guid Id, string Name);

    public sealed record IngredientUnitConvertionDto(Guid UnitToConvertId, double Ratio);

    public sealed record IngredientDto(Guid Id, string Name, NutritionalValueDto NutritionalValues, Guid BaseUnit, IEnumerable<IngredientUnitConvertionDto> IngredientUnitConvertions, IngredientCategoryDto ShoppingListCategory, IEnumerable<IngredientRecipeDto> Recipes, IEnumerable<IngredientCategoryDto> Categories);

    public sealed record Response(Guid Id, string Name, NutritionalValueDto NutritionalValues, Guid BaseUnit, IEnumerable<IngredientUnitConvertionDto> IngredientUnitConvertions, IngredientCategoryDto ShoppingListCategory, IEnumerable<HateoasResponse<IngredientRecipeDto>> Recipes, IEnumerable<IngredientCategoryDto> Categories);

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/{ingredientId}", Handler)
                     .WithName("GetIngredientById")
                     .WithDescription("Gets an ingredient by its ID");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, NotFound>> Handler(Request ingredientId, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, [FromServices] ICurrentUser currentUser, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IngredientId id = new(ingredientId.Id);

        Ingredient? ingredient = await GetIngredient(id, dbContext, cancellationToken)
                              ?? throw new EntityNotFoundException<Ingredient, IngredientId>(id);

        IEnumerable<IngredientRecipeDto> recipes = await GetIngredientRecipes(dbContext, ingredient, cancellationToken);
        IEnumerable<IngredientCategoryDto> categories = await GetIngredientCategories(dbContext, ingredient, cancellationToken);

        IngredientDto ingredientDto = MapToIngredientDto(
            ingredient,
            categories.FirstOrDefault(c => c.Id == ingredient.ShoppingListCategoryId?.Value),
            recipes,
            categories.Where(c => c.Id != ingredient.ShoppingListCategoryId?.Value)
        );

        bool isIngredientCreator = ingredient.CreatedBy == currentUser.Id;
        HateoasResponse<Response> hateoasResponse = MapToHateoasResponse(ingredientDto, hateoasBuilderFactory, isIngredientCreator);

        return TypedResults.Ok(hateoasResponse);
    }

    private static async Task<Ingredient?> GetIngredient(IngredientId ingredientId, IAppDbContext dbContext, CancellationToken cancellationToken)
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
        return ingredient.Categories.Any() || ingredient.ShoppingListCategoryId is not null
             ? await dbContext.IngredientCategories.AsNoTracking()
                                      .Where(c => ingredient.Categories.Contains(c.Id) || c.Id == ingredient.ShoppingListCategoryId)
                                      .Select(r => new IngredientCategoryDto(r.Id, r.Name))
                                      .ToListAsync(cancellationToken)
             : [];
    }

    private static IngredientDto MapToIngredientDto(Ingredient ingredient, IngredientCategoryDto? shoppingListCategory, IEnumerable<IngredientRecipeDto> recipes, IEnumerable<IngredientCategoryDto> categories)
    {
        return new IngredientDto(
            ingredient.Id.Value,
            ingredient.Name,
            MapToNutritionalValuesDto(ingredient.NutritionalValue),
            ingredient.BaseUnit.Value,
            ingredient.IngredientUnitConvertions.Select(c => new IngredientUnitConvertionDto(c.UnitToConvertId.Value, c.Ratio)),
            shoppingListCategory ?? default!,
            recipes,
            categories
        );
    }

    private static NutritionalValueDto MapToNutritionalValuesDto(NutritionalValue nutritionalValue)
    {
        return new NutritionalValueDto(nutritionalValue.Calories, nutritionalValue.Proteins, nutritionalValue.Fats, nutritionalValue.Carbohydrates, nutritionalValue.IngredientAmount, nutritionalValue.IngredientUnit);
    }

    private static HateoasResponse<Response> MapToHateoasResponse(IngredientDto ingredientDto, IHateoasBuilderFactory hateoasBuilderFactory, bool isIngredientCreator)
    {
        HateoasCollectionResponseBuilder<IngredientRecipeDto> recipesBuilder = hateoasBuilderFactory.ForCollection(ingredientDto.Recipes);

        recipesBuilder.WithCollectionLink()
                      .WithGet(LinkOptions.Create("GetRecipeById", HateoasRelConstants.Self, true), recipe => new { recipeId = recipe.Id });

        Response response = new(
            ingredientDto.Id,
            ingredientDto.Name,
            ingredientDto.NutritionalValues,
            ingredientDto.BaseUnit,
            ingredientDto.IngredientUnitConvertions,
            ingredientDto.ShoppingListCategory,
            recipesBuilder.Build().Items,
            ingredientDto.Categories
        );

        HateoasResponseBuilder<Response> responseBuilder = hateoasBuilderFactory.ForItem(response);

        responseBuilder.AddGet(LinkOptions.Create("GetIngredientById", HateoasRelConstants.Self, true), new { ingredientId = ingredientDto.Id })
                       .AddPut(LinkOptions.Create("UpdateIngredient", HateoasRelConstants.Update, isIngredientCreator), new { ingredientId = ingredientDto.Id })
                       .AddDelete(LinkOptions.Create("DeleteIngredient", HateoasRelConstants.Delete, isIngredientCreator), new { ingredientId = ingredientDto.Id });

        return responseBuilder.Build();
    }
}
