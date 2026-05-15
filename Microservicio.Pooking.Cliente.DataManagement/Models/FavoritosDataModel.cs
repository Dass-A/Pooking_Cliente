namespace Microservicio.Pooking.Cliente.DataManagement.Models;

public class FavoritosDataModel
{
    public int IdFavorito { get; set; }
    public Guid GuidFavorito { get; set; }
    public Guid GuidClienteRef { get; set; }
    public Guid GuidServicioRef { get; set; }
    public string? Alias { get; set; }
    public string Estado { get; set; } = "ACT";
    public string? CreadoPorUsuario { get; set; }
    public DateTimeOffset FechaRegistroUtc { get; set; }
}
