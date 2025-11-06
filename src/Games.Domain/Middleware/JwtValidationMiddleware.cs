using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Claims;


namespace Games.Domain.Middleware;


public class JwtValidationMiddleware
{
    private readonly RequestDelegate _next;

    public JwtValidationMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        if (context.Request.Path.StartsWithSegments("/swagger") ||
        context.Request.Path.StartsWithSegments("/health") || 
        context.Request.Path.StartsWithSegments("/api/v1/adicionar/jogo/biblioteca")) {
            await _next(context);
            return;
        }

        if (context.Request.Path.StartsWithSegments("/api/login") ||
            context.Request.Path.StartsWithSegments("/api/register")) {
            await _next(context);
            return;
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\": \"Token ausente ou inválido.\"}");
            return;
        }

        await _next(context);
    }
}

