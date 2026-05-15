using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Interfaces;

public interface IFavoritosDataService
{
    Task<FavoritosDataModel?> ObtenerPorGuidAsync(Guid guidFavorito, CancellationToken cancellationToken = default);

    Task<FavoritosDataModel?> ObtenerPorClienteYServicioAsync(
        Guid guidClienteRef,
        Guid guidServicioRef,
        CancellationToken cancellationToken = default);

    Task<PagedDataResult<FavoritosDataModel>> ListarPorClienteAsync(
        Guid guidClienteRef,
        int pagina,
        int tamanio,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteFavoritoAsync(
        Guid guidClienteRef,
        Guid guidServicioRef,
        CancellationToken cancellationToken = default);

    Task<FavoritosDataModel> CrearAsync(FavoritosDataModel favorito, CancellationToken cancellationToken = default);
    Task<FavoritosDataModel> ActualizarAsync(FavoritosDataModel favorito, CancellationToken cancellationToken = default);
    Task EliminarLogicoAsync(Guid guidFavorito, CancellationToken cancellationToken = default);
}
