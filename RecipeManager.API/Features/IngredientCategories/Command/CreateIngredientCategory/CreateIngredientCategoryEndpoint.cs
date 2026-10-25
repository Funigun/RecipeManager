using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.IngredientCategories.Command.CreateIngredientCategory;

public sealed class CreateIngredientCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPost<CreateIngredientCategoryRequest, CreateIngredientCategoryResponse>(
                     IngredientCategoryGroup.Route, Handle)
                 .WithName("CreateIngredientCategory")
                 .WithDescription("Creates a new ingredient category with the specified name.")
                 .WithTags(IngredientCategoryGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status403Forbidden)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Created<CreateIngredientCategoryResponse>, ValidationProblem, ProblemHttpResult>> Handle(
        CreateIngredientCategoryRequest request,
        [FromServices] CreateIngredientCategoryCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<CreateIngredientCategoryResponse> result = await commandHandler.Handle(
            new CreateIngredientCategoryCommand(request), cancellationToken);
        return result.ToHttpResult(response =>
            TypedResults.Created(IngredientCategoryGroup.CategoryById(response.Id.Value), response));
    }
}
