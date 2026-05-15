namespace Microservicio.Pooking.Cliente.Api.Models.Common;

public sealed class ApiErrorResponse
{
    public bool Success { get; init; } = false;
    public string Message { get; init; } = string.Empty;
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static ApiErrorResponse Crear(string message, IReadOnlyList<string>? errors = null) =>
        new()
        {
            Message = message,
            Errors = errors ?? Array.Empty<string>()
        };
}
