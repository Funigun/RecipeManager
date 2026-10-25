using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.RecipeCategories.Command.CreateRecipeCategory;

public sealed class CreateRecipeCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPost<CreateRecipeCategoryRequest, CreateRecipeCategoryResponse>(
                     RecipeCategoriesGroup.Route, Handle)
                 .WithName("CreateRecipeCategory")
                 .WithDescription("Creates a new recipe category with the specified name.")
                 .WithTags(RecipeCategoriesGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status403Forbidden)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Created<CreateRecipeCategoryResponse>, ValidationProblem, ProblemHttpResult>> Handle(
        CreateRecipeCategoryRequest request,
        [FromServices] CreateRecipeCategoryCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<CreateRecipeCategoryResponse> result = await commandHandler.Handle(
            new CreateRecipeCategoryCommand(request), cancellationToken);
        return result.ToHttpResult(response =>
            TypedResults.Created(RecipeCategoriesGroup.CategoryById(response.Id.Value), response));
    }
}
