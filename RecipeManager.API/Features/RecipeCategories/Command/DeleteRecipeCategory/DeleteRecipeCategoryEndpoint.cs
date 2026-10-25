using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.RecipeCategories.Command.DeleteRecipeCategory;

public sealed class DeleteRecipeCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardDelete<DeleteRecipeCategoryCommand>(RecipeCategoriesGroup.CategoryByIdRoute, Handle)
                 .WithName("DeleteRecipeCategory")
                 .WithDescription("Deletes a recipe category")
                 .WithTags(RecipeCategoriesGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized)
                 .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid categoryId,
        [FromServices] DeleteRecipeCategoryCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<RecipeCategoryId> result = await commandHandler.Handle(
            new DeleteRecipeCategoryCommand(categoryId), cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
