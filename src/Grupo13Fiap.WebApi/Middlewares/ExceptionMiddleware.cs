using Grupo13Fiap.Domain.Exceptions;
using Grupo13Fiap.WebApi.Controllers.Shared;

using System.Net;
using System.Text.Json;

using DomainValidationException = Grupo13Fiap.Domain.Exceptions.ValidationException;

namespace Grupo13Fiap.WebApi.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;
    private readonly IHostEnvironment _env = env;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        } catch(Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, problemDetails) = MapException(exception, context);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, _jsonSerializerOptions));
    }

    private (HttpStatusCode statusCode, CustomProblemDetails problemDetails) MapException(Exception exception, HttpContext context)
    {
        return exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                new CustomProblemDetails(HttpStatusCode.NotFound, context.Request, notFound.Message)),

            DomainValidationException validation => (
                HttpStatusCode.BadRequest,
                new CustomProblemDetails(HttpStatusCode.BadRequest, context.Request, errors: validation.Errors)),

            ConflictException conflict => (
                HttpStatusCode.Conflict,
                new CustomProblemDetails(HttpStatusCode.Conflict, context.Request, conflict.Message)),

            DomainException domain => (
                HttpStatusCode.BadRequest,
                new CustomProblemDetails(HttpStatusCode.BadRequest, context.Request, domain.Message)),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new CustomProblemDetails(HttpStatusCode.Unauthorized, context.Request)),

            ArgumentException argument => (
                HttpStatusCode.BadRequest,
                new CustomProblemDetails(HttpStatusCode.BadRequest, context.Request, argument.Message)),

            NotImplementedException => (
                HttpStatusCode.NotImplemented,
                new CustomProblemDetails(HttpStatusCode.NotImplemented, context.Request)),

            _ => (
                HttpStatusCode.InternalServerError,
                new CustomProblemDetails(
                    HttpStatusCode.InternalServerError,
                    context.Request,
                    _env.IsDevelopment() || _env.IsStaging() ? exception.Message : null))
        };
    }
}
