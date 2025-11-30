using System.Text.Json;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Components.Extensions;

public static class ExceptionExtensions
{
    public static string ToStringArray(this HttpResponseMessage message)
    {
        int statusCode = (int)message.StatusCode;

        ApiResponseBody body = message.Content.ReadFromJsonAsync<ApiResponseBody>().GetAwaiter().GetResult()
                             ?? new()
                             {
                                 StatusCode = statusCode,
                                 Message = "An error occurred while processing your request.",
                                 Errors = [],
                                 ValidationErrors = []
                             };

        return JsonSerializer.Serialize(body);
    }


}
