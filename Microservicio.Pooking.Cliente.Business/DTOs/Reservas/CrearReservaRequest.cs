namespace Microservicio.Pooking.Cliente.Business.DTOs.Reservas;

/// <summary>
/// Request para crear una reserva.
/// El snapshot del servicio (nombre, tipo) DEBE proporcionarse desde fuera,
/// idealmente obtenido vía gRPC al microservicio Catálogo. Por ahora viene en el request.
/// </summary>
public class CrearReservaRequest
{
    public Guid GuidCliente { get; set; }
    public Guid GuidServicioRef { get; set; }

    /// <summary>Snapshot del nombre del servicio en el momento de la reserva.</summary>
    public string NombreServicioSnap { get; set; } = string.Empty;

    /// <summary>Snapshot del tipo de servicio (Vuelos | Alojamiento | Atracciones | Alquiler de Carros).</summary>
    public string TipoServicioSnap { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del proveedor que opera la reserva (ej. "Avianca", "Hotel Sangay", "Localiza").
    /// Corresponde a la razón social del servicio en el dominio Servicio.
    /// </summary>
    public string NombreProveedor { get; set; } = string.Empty;

    public string IdReservaExterna { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? CanalOrigen { get; set; }
    public decimal MontoTotal { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? Observaciones { get; set; }
}
