namespace Microservicio.Pooking.Cliente.Business.DTOs.Cliente;

public class ClienteResponse
{
    public Guid GuidCliente { get; set; }
    public Guid UsuarioGuidRef { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? RazonSocial { get; set; }
    public string TipoIdentificacion { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTimeOffset FechaRegistroUtc { get; set; }
    public DateTimeOffset? FechaModificacionUtc { get; set; }
}
