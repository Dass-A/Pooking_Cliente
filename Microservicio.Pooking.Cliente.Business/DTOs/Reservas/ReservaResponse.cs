namespace Microservicio.Pooking.Cliente.Business.DTOs.Reservas;

public class ReservaResponse
{
    public Guid GuidReserva { get; set; }
    public Guid GuidClienteRef { get; set; }
    public Guid GuidServicioRef { get; set; }
    public string NombreServicioSnap { get; set; } = string.Empty;
    public string TipoServicioSnap { get; set; } = string.Empty;
    public string NombreProveedor { get; set; } = string.Empty;
    public string IdReservaExterna { get; set; } = string.Empty;
    public DateTimeOffset FechaReservaUtc { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? CanalOrigen { get; set; }
    public decimal MontoTotal { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? Observaciones { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? MotivoCancelacion { get; set; }
    public DateTimeOffset? FechaCancelacionUtc { get; set; }
}
