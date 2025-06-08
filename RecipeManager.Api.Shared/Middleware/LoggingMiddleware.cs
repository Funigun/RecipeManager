using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RecipeManager.Api.Shared.Middleware;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Handling request");

        await next(context);

        logger.LogInformation("Finished handling request");
    }
}
