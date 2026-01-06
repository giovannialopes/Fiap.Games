using Games.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Games.Domain.Services.Class;

/// <summary>
/// Serviço de logging de requisições (SRP - Single Responsibility)
/// </summary>
public class RequestLogger : IRequestLogger
{
    private readonly ILogger<RequestLogger> _logger;

    public RequestLogger(ILogger<RequestLogger> logger)
    {
        _logger = logger;
    }

    public void LogRequest(string traceId, string method, string path, string? remoteIp, string? userId)
    {
        _logger.LogInformation(
            "[{TraceId}] Incoming request: {Method} {Path} from {RemoteIp} | User: {UserId}",
            traceId, method, path, remoteIp, userId);
    }

    public void LogResponse(string traceId, string method, string path, int statusCode, long durationMs, string? userId)
    {
        // KISS - Lógica simples e direta
        if (statusCode >= 200 && statusCode < 300)
        {
            _logger.LogInformation(
                "[{TraceId}] Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms | User: {UserId}",
                traceId, method, path, statusCode, durationMs, userId);
        }
        else if (statusCode >= 400 && statusCode < 500)
        {
            _logger.LogWarning(
                "[{TraceId}] Client error: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms | User: {UserId}",
                traceId, method, path, statusCode, durationMs, userId);
        }
        else if (statusCode >= 500)
        {
            _logger.LogError(
                "[{TraceId}] Server error: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms | User: {UserId}",
                traceId, method, path, statusCode, durationMs, userId);
        }
    }

    public void LogError(string traceId, string method, string path, long durationMs, string? userId, Exception ex)
    {
        _logger.LogError(ex,
            "[{TraceId}] Request error: {Method} {Path} - Duration: {Duration}ms | User: {UserId} | Error: {ErrorMessage}",
            traceId, method, path, durationMs, userId, ex.Message);
    }
}

