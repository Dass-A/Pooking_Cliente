using System.Net;
using System.Text.Json;
using Microservicio.Pooking.Cliente.Api.Models.Common;
using Microservicio.Pooking.Cliente.Business.Exceptions;

namespace Microservicio.Pooking.Cliente.Api.Middleware;

/// <summary>
/// Middleware global que traduce excepciones de negocio a respuestas HTTP estándar.
/// Debe ser el PRIMERO del pipeline.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado: {Mensaje}", ex.Message);
            await EscribirRespuestaAsync(context, ex);
        }
    }

    private static async Task EscribirRespuestaAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, body) = exception switch
        {
            ValidationException ve => (
                (int)HttpStatusCode.BadRequest,
                ApiErrorResponse.Crear(ve.Message, ve.Errors.ToArray())),
            NotFoundException ne => (
                (int)HttpStatusCode.NotFound,
                ApiErrorResponse.Crear(ne.Message, new[] { ne.Message })),
            BusinessException be => (
                (int)HttpStatusCode.Conflict,
                ApiErrorResponse.Crear(be.Message, new[] { be.Message })),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                ApiErrorResponse.Crear("Error interno del servidor.", Array.Empty<string>()))
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
