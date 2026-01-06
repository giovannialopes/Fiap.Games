using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Games.Domain.Services.Interface;

namespace Games.Domain.Middleware;

/// <summary>
/// Middleware para observabilidade (SRP - Single Responsibility: apenas orquestra observabilidade)
/// </summary>
public class ObservabilityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRequestLogger _requestLogger;
    private static readonly string[] _ignoredPaths = { "/health", "/metrics", "/swagger" };

    public ObservabilityMiddleware(RequestDelegate next, IRequestLogger requestLogger)
    {
        _next = next;
        _requestLogger = requestLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";
        var traceId = context.TraceIdentifier;
        var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        
        // KISS - Lógica simples para ignorar paths
        if (ShouldIgnorePath(path))
        {
            await _next(context);
            return;
        }

        try
        {
            _requestLogger.LogRequest(traceId, method, path, context.Connection.RemoteIpAddress?.ToString(), userId);

            await _next(context);

            stopwatch.Stop();
            _requestLogger.LogResponse(traceId, method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, userId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _requestLogger.LogError(traceId, method, path, stopwatch.ElapsedMilliseconds, userId, ex);

            if (!context.Response.HasStarted)
            {
                await WriteErrorResponse(context, traceId);
            }
        }
    }

    // DRY - Método auxiliar para evitar duplicação
    private static bool ShouldIgnorePath(string path)
    {
        return _ignoredPaths.Any(ignored => path.StartsWith(ignored, StringComparison.OrdinalIgnoreCase));
    }

    // DRY - Método auxiliar para resposta de erro
    private static async Task WriteErrorResponse(HttpContext context, string traceId)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var response = JsonSerializer.Serialize(new
        {
            error = "Internal Server Error",
            traceId = traceId,
            timestamp = DateTime.UtcNow
        });
        await context.Response.WriteAsync(response);
    }
}


