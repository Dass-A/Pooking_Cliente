namespace Microservicio.Pooking.Cliente.DataManagement.Models;

/// <summary>
/// Modelo de datos intermedio entre DataAccess (entidad) y Business (DTO).
/// Aísla el Business de los detalles del ORM.
/// </summary>
public class ClienteDataModel
{
    public int IdCliente { get; set; }
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
    public string Estado { get; set; } = "ACT";
    public bool EsEliminado { get; set; }
    public string? CreadoPorUsuario { get; set; }
    public DateTimeOffset FechaRegistroUtc { get; set; }
    public string? ModificadoPorUsuario { get; set; }
    public DateTimeOffset? FechaModificacionUtc { get; set; }
}
