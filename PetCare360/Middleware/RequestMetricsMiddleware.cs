using System.Diagnostics;

namespace PetCare360.Middleware;

public class RequestMetricsMiddleware
{
    private readonly RequestDelegate _next;

    public RequestMetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ApplicationMetrics metrics)
    {
        var stopwatch = Stopwatch.StartNew();

        metrics.Requests.Add(
            1,
            new KeyValuePair<string, object?>(
                "method",
                context.Request.Method));

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;

            metrics.RequestDuration.Record(
                stopwatch.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>(
                    "method",
                    context.Request.Method),
                new KeyValuePair<string, object?>(
                    "status_code",
                    statusCode));

            if (statusCode >= 400)
            {
                metrics.Errors.Add(
                    1,
                    new KeyValuePair<string, object?>(
                        "method",
                        context.Request.Method),
                    new KeyValuePair<string, object?>(
                        "status_code",
                        statusCode));
            }
        }
    }
}