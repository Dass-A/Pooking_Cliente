namespace Microservicio.Pooking.Cliente.DataAccess.Entities;

/// <summary>
/// Entidad que mapea la tabla booking.reservas en PostgreSQL.
/// Historial completo de reservas por cliente.
/// El snapshot del servicio (nombre, tipo) garantiza trazabilidad histórica
/// ante cambios posteriores en el dominio Servicio.
/// </summary>
public class ReservasEntity
{
    // -------------------------------------------------------------------------
    // [1] Identificación técnica
    // -------------------------------------------------------------------------

    /// <summary>Clave primaria interna (BIGSERIAL). No se expone en la API.</summary>
    public long IdReserva { get; set; }

    /// <summary>Identificador público UUID expuesto en la API REST.</summary>
    public Guid GuidReserva { get; set; }

    // -------------------------------------------------------------------------
    // [2] FK interna al dominio Cliente
    // -------------------------------------------------------------------------

    public int IdCliente { get; set; }

    /// <summary>Navegación al cliente dueño de la reserva.</summary>
    public ClienteEntity? Cliente { get; set; }

    // -------------------------------------------------------------------------
    // [3] Referencia lógica al dominio Servicio (sin FK física cross-dominio)
    // -------------------------------------------------------------------------

    /// <summary>
    /// UUID del servicio en el dominio Servicio.
    /// Referencia lógica, sin FK física cross-dominio.
    /// </summary>
    public Guid GuidServicioRef { get; set; }

    // -------------------------------------------------------------------------
    // [4] Snapshot inmutable del servicio en el momento de la reserva
    // -------------------------------------------------------------------------

    /// <summary>Nombre del servicio capturado al momento de la reserva (inmutable).</summary>
    public string NombreServicioSnap { get; set; } = string.Empty;

    /// <summary>Tipo de servicio capturado al momento de la reserva (inmutable).</summary>
    public string TipoServicioSnap { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del proveedor que opera la reserva.
    /// Corresponde a booking.servicio.razon_social en el dominio Servicio.
    /// Necesario para saber a qué proveedor consultar el estado de la reserva.
    /// </summary>
    public string NombreProveedor { get; set; } = string.Empty;

    // -------------------------------------------------------------------------
    // [5] Identificador externo del proveedor / sistema origen
    // -------------------------------------------------------------------------

    /// <summary>Identificador asignado por el sistema/proveedor externo.</summary>
    public string IdReservaExterna { get; set; } = string.Empty;

    // -------------------------------------------------------------------------
    // [6] Datos funcionales de la reserva
    // -------------------------------------------------------------------------

    public DateTimeOffset FechaReservaUtc { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? CanalOrigen { get; set; }
    public decimal MontoTotal { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? Observaciones { get; set; }

    // -------------------------------------------------------------------------
    // [7] Estado y ciclo de vida
    // -------------------------------------------------------------------------

    /// <summary>PEND = Pendiente | CONF = Confirmada | CANC = Cancelada | COMP = Completada.</summary>
    public string Estado { get; set; } = "CONF";
    public string? MotivoCancelacion { get; set; }
    public DateTimeOffset? FechaCancelacionUtc { get; set; }
    public bool EsEliminado { get; set; } = false;

    // -------------------------------------------------------------------------
    // [8] Auditoría
    // -------------------------------------------------------------------------

    public string? CreadoPorUsuario { get; set; }
    public DateTimeOffset FechaRegistroUtc { get; set; }
    public string? ModificadoPorUsuario { get; set; }
    public DateTimeOffset? FechaModificacionUtc { get; set; }
    public string? ModificacionIp { get; set; }
    public string? ServicioOrigen { get; set; }
}
