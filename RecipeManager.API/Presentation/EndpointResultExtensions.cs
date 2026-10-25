using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Application;

namespace RecipeManager.Api.Presentation;

public static class EndpointResultExtensions
{
    public static Results<TSuccess, ValidationProblem, ProblemHttpResult> ToHttpResult<TValue, TSuccess>(this Result<TValue> result, Func<TValue, TSuccess> onSuccess)
            where TSuccess : IResult
    {
        return result switch
        {
            TValue value => onSuccess(value),

            ValidationError error => TypedResults.ValidationProblem(
                error.Errors.ToDictionary(entry => entry.Key, entry => entry.Value),
                title: "Validation failed"),

            AuthorizationError => TypedResults.Problem(
                detail: "You are not authorized to perform this action.",
                statusCode: StatusCodes.Status403Forbidden),

            NotFoundError => TypedResults.Problem(
                detail: "The requested resource was not found.",
                statusCode: StatusCodes.Status404NotFound)
        };
    }
}
