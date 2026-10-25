using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.IngredientCategories.Command.DeleteIngredientCategory;

public sealed class DeleteIngredientCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardDelete<DeleteIngredientCategoryCommand>(
                     IngredientCategoryGroup.CategoryByIdRoute, Handle)
                 .WithName("DeleteIngredientCategory")
                 .WithDescription("Deletes an ingredient category")
                 .WithTags(IngredientCategoryGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized)
                 .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid categoryId,
        [FromServices] DeleteIngredientCategoryCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<IngredientCategoryId> result = await commandHandler.Handle(
            new DeleteIngredientCategoryCommand(categoryId), cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}
