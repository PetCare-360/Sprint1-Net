using System.Net;
using System.Text.Json;
using PetCare360.Exceptions;

namespace PetCare360.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException e => (HttpStatusCode.NotFound, e.Message),
            BadRequestException e => (HttpStatusCode.BadRequest, e.Message),
            UnauthorizedException e => (HttpStatusCode.Unauthorized, e.Message),
            ConflictException e => (HttpStatusCode.Conflict, e.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            error = statusCode.ToString(),
            message,
            path = context.Request.Path.Value,
            timestamp = DateTimeOffset.UtcNow
        });

        return context.Response.WriteAsync(body);
    }
}