namespace Microservicio.Pooking.Cliente.Business.DTOs.Cliente;

/// <summary>
/// Request para crear un nuevo cliente.
/// El UsuarioGuidRef debe corresponder a un usuario ya existente en el dominio Auth.
/// La validación de existencia es responsabilidad del Auth Service (vía gRPC en el futuro).
/// </summary>
public class CrearClienteRequest
{
    /// <summary>UUID del usuario en el dominio Auth (referencia lógica).</summary>
    public Guid UsuarioGuidRef { get; set; }

    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? RazonSocial { get; set; }

    /// <summary>CI | RUC | PASS | EXT</summary>
    public string TipoIdentificacion { get; set; } = string.Empty;

    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
