using Microsoft.AspNetCore.Http;
using System.Text.Json;
using static Games.Domain.DTO.ErrorDto;

namespace Games.Domain.Middleware;

public class ErrorsMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorsMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized) {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse {
                StatusCode = 401,
                Message = "Token ausente ou inválido."
            }));
        }
        else if (context.Response.StatusCode == StatusCodes.Status403Forbidden) { 
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse {
                StatusCode = 403,
                Message = "Você não tem permissão para acessar este recurso."
            }));
        }
    }
}

