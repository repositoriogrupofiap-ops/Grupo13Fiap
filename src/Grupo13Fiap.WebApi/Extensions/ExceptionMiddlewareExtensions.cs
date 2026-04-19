using Grupo13Fiap.WebApi.Middlewares;

namespace Grupo13Fiap.WebApi.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}
