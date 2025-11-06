using Microsoft.AspNetCore.Http;
using System.Text;

namespace Games.Domain.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/health")) {
                await _next(context);
                return;
            }

            if (context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put ||
                context.Request.Method == HttpMethods.Patch) {

                if (context.Request.ContentLength == 0) {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("O corpo da requisição não pode estar vazio.");
                    return;
                }

                try {
                    context.Request.EnableBuffering();

                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (string.IsNullOrWhiteSpace(body)) {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("O corpo da requisição não pode estar vazio.");
                        return;
                    }
                }
                catch (Exception ex) {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync($"Erro ao processar o corpo da requisição: {ex.Message}");
                    return;
                }
            }

            await _next(context);
        }

    }
}
