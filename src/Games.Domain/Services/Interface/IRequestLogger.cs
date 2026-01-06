namespace Games.Domain.Services.Interface;

/// <summary>
/// Interface para logging de requisições (ISP - Interface Segregation)
/// </summary>
public interface IRequestLogger
{
    void LogRequest(string traceId, string method, string path, string? remoteIp, string? userId);
    void LogResponse(string traceId, string method, string path, int statusCode, long durationMs, string? userId);
    void LogError(string traceId, string method, string path, long durationMs, string? userId, Exception ex);
}

