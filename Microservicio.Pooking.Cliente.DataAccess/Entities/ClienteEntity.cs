namespace Microservicio.Pooking.Cliente.DataAccess.Entities;

/// <summary>
/// Entidad que mapea la tabla booking.cliente en PostgreSQL.
/// Referencia lógica 1:1 con el dominio Auth vía UsuarioGuidRef (UUID).
/// SIN FK física cross-dominio: el dominio Auth vive en otro microservicio.
/// </summary>
public class ClienteEntity
{
    // -------------------------------------------------------------------------
    // [1] Identificación técnica
    // -------------------------------------------------------------------------

    /// <summary>Clave primaria interna (SERIAL). No se expone en la API.</summary>
    public int IdCliente { get; set; }

    /// <summary>Identificador público UUID expuesto en la API REST.</summary>
    public Guid GuidCliente { get; set; }

    // -------------------------------------------------------------------------
    // [2] Referencia lógica al dominio Auth (sin FK física cross-dominio)
    // -------------------------------------------------------------------------

    /// <summary>
    /// UUID del usuario en el dominio Auth.
    /// Referencia lógica, sin FK física cross-dominio.
    /// Para resolver el usuario se llama al microservicio Auth por gRPC/REST.
    /// </summary>
    public Guid UsuarioGuidRef { get; set; }

    // -------------------------------------------------------------------------
    // [3] Datos funcionales
    // -------------------------------------------------------------------------

    /// <summary>Nombre(s) del cliente. Solo para personas naturales.</summary>
    public string? Nombres { get; set; }

    /// <summary>Apellido(s) del cliente. Solo para personas naturales.</summary>
    public string? Apellidos { get; set; }

    /// <summary>
    /// Razón social. Solo para personas jurídicas (tipo_identificacion = 'RUC').
    /// Al menos uno de Nombres o RazonSocial debe estar presente (validación en negocio).
    /// </summary>
    public string? RazonSocial { get; set; }

    /// <summary>
    /// Tipo de documento de identidad.
    /// Valores permitidos: CI | RUC | PASS | EXT
    /// </summary>
    public string TipoIdentificacion { get; set; } = string.Empty;

    /// <summary>Número del documento de identidad. Único por tipo.</summary>
    public string NumeroIdentificacion { get; set; } = string.Empty;

    /// <summary>Correo electrónico del cliente. Indexado en BD.</summary>
    public string Correo { get; set; } = string.Empty;

    /// <summary>Teléfono de contacto. Opcional.</summary>
    public string? Telefono { get; set; }

    /// <summary>Dirección del cliente. Opcional.</summary>
    public string? Direccion { get; set; }

    // -------------------------------------------------------------------------
    // [4] Estado y ciclo de vida
    // -------------------------------------------------------------------------

    /// <summary>Valores: ACT = Activo | INA = Inactivo | SUS = Suspendido.</summary>
    public string Estado { get; set; } = "ACT";

    /// <summary>Soft delete. No se eliminan registros físicamente.</summary>
    public bool EsEliminado { get; set; } = false;

    // -------------------------------------------------------------------------
    // [5] Auditoría
    // -------------------------------------------------------------------------

    public string? CreadoPorUsuario { get; set; }
    public DateTimeOffset FechaRegistroUtc { get; set; }
    public string? ModificadoPorUsuario { get; set; }
    public DateTimeOffset? FechaModificacionUtc { get; set; }
    public string? ModificacionIp { get; set; }
    public string? ServicioOrigen { get; set; }
}
