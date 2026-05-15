namespace Microservicio.Pooking.Cliente.Api.Models.Settings;

/// <summary>
/// Configuración de los endpoints gRPC remotos que este microservicio consume.
/// Se lee desde la sección "GrpcEndpoints" en appsettings.json.
/// </summary>
public sealed class GrpcEndpointsSettings
{
    public const string SectionName = "GrpcEndpoints";

    /// <summary>
    /// URL del Booking.Middleware (único microservicio gRPC consumido directamente).
    /// En desarrollo: https://localhost:5200
    /// En producción: variable de entorno o service discovery.
    /// </summary>
    public string Middleware { get; set; } = string.Empty;

    /// <summary>
    /// Si false, el cliente gRPC se inyecta como stub no-op (devuelve respuestas vacías
    /// sin lanzar errores). Útil para desarrollo cuando el Middleware no está corriendo.
    /// </summary>
    public bool Enabled { get; set; } = false;
}
