namespace Microservicio.Pooking.Cliente.Business.DTOs.Favoritos;

public class FavoritoResponse
{
    public Guid GuidFavorito { get; set; }
    public Guid GuidClienteRef { get; set; }
    public Guid GuidServicioRef { get; set; }
    public string? Alias { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTimeOffset FechaRegistroUtc { get; set; }
}
