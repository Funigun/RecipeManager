using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.API.Features.Units;

public static class DeleteUnit
{
    [GroupEndpoint("Units")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("", Handler)
                     .WithName("DeleteMeasurementUnit")
                     .WithDescription("Deletes a measurement unit");
        }
    }

    internal static async Task<Results<Ok<string>, NotFound>> Handler()
    {
        // Logic for changing the password goes here
        return TypedResults.Ok("Measurement unit deleted succesfully");
    }
}
