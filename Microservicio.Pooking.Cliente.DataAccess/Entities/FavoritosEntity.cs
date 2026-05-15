namespace Microservicio.Pooking.Cliente.DataAccess.Entities;

/// <summary>
/// Entidad que mapea la tabla booking.favoritos en PostgreSQL.
/// Vincula un cliente con un servicio marcado como favorito.
/// Ambas referencias son lógicas (UUID cross-dominio, sin FK física).
/// </summary>
public class FavoritosEntity
{
    // -------------------------------------------------------------------------
    // [1] Identificación técnica
    // -------------------------------------------------------------------------

    public int IdFavorito { get; set; }
    public Guid GuidFavorito { get; set; }

    // -------------------------------------------------------------------------
    // [2] Referencias lógicas cross-dominio (sin FK física)
    // -------------------------------------------------------------------------

    /// <summary>UUID del cliente; referencia lógica a booking.cliente.guid_cliente.</summary>
    public Guid GuidClienteRef { get; set; }

    /// <summary>UUID del servicio; referencia lógica al dominio Servicio.</summary>
    public Guid GuidServicioRef { get; set; }

    // -------------------------------------------------------------------------
    // [3] Datos funcionales
    // -------------------------------------------------------------------------

    /// <summary>Etiqueta personalizada opcional asignada por el cliente.</summary>
    public string? Alias { get; set; }

    // -------------------------------------------------------------------------
    // [4] Estado y ciclo de vida
    // -------------------------------------------------------------------------

    /// <summary>ACT = Activo | INA = Inactivo.</summary>
    public string Estado { get; set; } = "ACT";
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
