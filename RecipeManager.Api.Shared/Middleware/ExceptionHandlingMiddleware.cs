using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecipeManager.Api.Shared.Contracts.Exceptions;
using System.Net;
using System.Text.Json;

namespace RecipeManager.Api.Shared.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        ResponseBody response = ToResponseBody(exception);

        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = "application/json";

        string jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }

    private ResponseBody ToResponseBody(Exception exception)
    {
        return exception switch
        {
            NotFoundException => new ResponseBody
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = exception.Message,
            },

            UnauthorizedAccessException => new ResponseBody
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = "You are not authorized to perform this action",
            },

            ValidationException e => new ResponseBody
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation failed",
                Errors = e.Errors.ToList()
            },

            ApplicationValidationException e => new ResponseBody
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation failed",
                Errors = e.Errors.ToList()
            },

            _ => new ResponseBody
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred. Please try again later.",
            }
        };
    }
}