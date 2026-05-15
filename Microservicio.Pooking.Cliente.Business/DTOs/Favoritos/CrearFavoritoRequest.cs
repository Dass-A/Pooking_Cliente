namespace Microservicio.Pooking.Cliente.Business.DTOs.Favoritos;

/// <summary>
/// Request para marcar un servicio como favorito.
/// La validación de existencia del servicio (vía gRPC al Catálogo) es un TODO futuro.
/// </summary>
public class CrearFavoritoRequest
{
    public Guid GuidClienteRef { get; set; }
    public Guid GuidServicioRef { get; set; }
    public string? Alias { get; set; }
}
