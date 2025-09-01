using System.Diagnostics;

namespace MinimalApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        _logger.LogInformation(
            "Iniciando requisição {Method} {Path} de {IP} - UserAgent: {UserAgent}",
            requestMethod, requestPath, ipAddress, userAgent);

        try
        {
            await _next(context);
            
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            
            _logger.LogInformation(
                "Requisição {Method} {Path} concluída com status {StatusCode} em {ElapsedMs}ms",
                requestMethod, requestPath, statusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex,
                "Erro na requisição {Method} {Path} após {ElapsedMs}ms",
                requestMethod, requestPath, stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}
