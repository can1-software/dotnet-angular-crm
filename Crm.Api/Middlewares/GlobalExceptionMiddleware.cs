using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Crm.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                validationException.Errors
                    .Select(error => new { field = error.PropertyName, message = error.ErrorMessage })
                    .ToArray<object>()),
            KeyNotFoundException keyNotFoundException => (
                HttpStatusCode.NotFound,
                keyNotFoundException.Message,
                Array.Empty<object>()),
            UnauthorizedAccessException unauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                unauthorizedAccessException.Message,
                Array.Empty<object>()),
            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                Array.Empty<object>())
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "An unhandled exception occurred.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            success = false,
            message,
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
