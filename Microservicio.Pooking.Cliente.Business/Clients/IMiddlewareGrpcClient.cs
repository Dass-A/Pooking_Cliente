namespace Microservicio.Pooking.Cliente.Business.Clients;

/// <summary>
/// Cliente único hacia el Booking.Middleware.
///
/// Encapsula las dos responsabilidades del Middleware:
///   - Resolver (SÍNCRONO): obtener datos de otros dominios (Servicio, Auth).
///   - EventBus (ASÍNCRONO): publicar eventos de auditoría.
///
/// Esta interfaz aísla al resto del código de los tipos generados por Protobuf,
/// y permite mockear fácilmente en tests.
///
/// IMPORTANTE: Cliente NO llama directamente a Auth, Servicio ni Auditoria.
/// Toda comunicación cross-dominio pasa por el Middleware (regla arquitectónica del equipo).
/// </summary>
public interface IMiddlewareGrpcClient
{
    // -------------------------------------------------------------------------
    // Resolver — síncrono
    // -------------------------------------------------------------------------

    /// <summary>
    /// Resuelve los datos de un servicio (proveedor) por su GUID.
    /// El Middleware internamente llama al Microservicio.Pooking.Servicio.
    /// </summary>
    /// <returns>Datos del servicio, o null si no existe.</returns>
    Task<ServicioResueltoDto?> ResolverServicioAsync(
        Guid guidServicio,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida disponibilidad de un servicio para un rango de fechas.
    /// </summary>
    Task<DisponibilidadResueltaDto> ValidarDisponibilidadAsync(
        Guid guidServicio,
        DateTime fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default);

    // -------------------------------------------------------------------------
    // EventBus — asíncrono (fire-and-forget en términos de negocio)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Publica un evento de auditoría. El Middleware lo rutea a Auditoria.
    /// No bloquea el flujo de negocio si falla — el error se loggea pero no se propaga.
    /// </summary>
    Task PublicarEventoAsync(
        EventoAuditoriaDto evento,
        CancellationToken cancellationToken = default);
}

// -----------------------------------------------------------------------------
// DTOs del dominio Cliente (no exponen tipos generados de Protobuf)
// -----------------------------------------------------------------------------

public class ServicioResueltoDto
{
    public Guid GuidServicio { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string TipoServicio { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CorreoContacto { get; set; } = string.Empty;
    public string TelefonoContacto { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
}

public class DisponibilidadResueltaDto
{
    public bool Disponible { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

public class EventoAuditoriaDto
{
    public string IdCorrelacion { get; set; } = Guid.NewGuid().ToString();
    public string TablaAfectada { get; set; } = string.Empty;
    public string EsquemaAfectado { get; set; } = "booking";
    public TipoOperacionAuditoria Operacion { get; set; }
    public string IdRegistro { get; set; } = string.Empty;
    public string DatosAnteriores { get; set; } = string.Empty;
    public string DatosNuevos { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string ServicioOrigen { get; set; } = "Cliente";
    public string EquipoOrigen { get; set; } = string.Empty;
}

public enum TipoOperacionAuditoria
{
    Insert = 0,
    Update = 1,
    Delete = 2
}
