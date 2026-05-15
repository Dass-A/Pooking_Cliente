namespace Microservicio.Pooking.Cliente.Api.Models.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> Exitoso(T data, string message = "Operación exitosa") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> SinContenido(string message = "Sin datos") =>
        new() { Success = true, Message = message, Data = default };

    public static ApiResponse<T> Ok(T data, string message = "Operación exitosa") =>
        Exitoso(data, message);
}
