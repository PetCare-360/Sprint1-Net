using Microsoft.AspNetCore.Http;
using PetCare360.Exceptions;
using Serilog;

namespace PetCare360.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            Log.Warning(ex, "Recurso não encontrado: {Message}", ex.Message);

            await WriteResponseAsync(
                context,
                StatusCodes.Status404NotFound,
                ex.Message);
        }
        catch (BadRequestException ex)
        {
            Log.Warning(ex, "Requisição inválida: {Message}", ex.Message);

            await WriteResponseAsync(
                context,
                StatusCodes.Status400BadRequest,
                ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            Log.Warning(ex, "Acesso não autorizado: {Message}", ex.Message);

            await WriteResponseAsync(
                context,
                StatusCodes.Status401Unauthorized,
                ex.Message);
        }
        catch (ConflictException ex)
        {
            Log.Warning(ex, "Conflito: {Message}", ex.Message);

            await WriteResponseAsync(
                context,
                StatusCodes.Status409Conflict,
                ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro interno não tratado");

            await WriteResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Ocorreu um erro interno no servidor.");
        }
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode,
            message,
            timestamp = DateTime.UtcNow,
            traceId = context.TraceIdentifier
        });
    }
}