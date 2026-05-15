using Microservicio.Pooking.Cliente.DataAccess.Common;
using Microservicio.Pooking.Cliente.DataAccess.Entities;

namespace Microservicio.Pooking.Cliente.DataAccess.Repositories.Interfaces;

public interface IFavoritosRepository
{
    // Lecturas
    Task<FavoritosEntity?> ObtenerPorGuidAsync(Guid guidFavorito, CancellationToken cancellationToken = default);

    Task<FavoritosEntity?> ObtenerPorClienteYServicioAsync(
        Guid guidClienteRef,
        Guid guidServicioRef,
        CancellationToken cancellationToken = default);

    Task<PagedResult<FavoritosEntity>> ObtenerPorClientePaginadoAsync(
        Guid guidClienteRef,
        int paginaActual,
        int tamanioPagina,
        CancellationToken cancellationToken = default);

    // Verificaciones
    Task<bool> ExisteFavoritoAsync(
        Guid guidClienteRef,
        Guid guidServicioRef,
        CancellationToken cancellationToken = default);

    // Escritura
    Task AgregarAsync(FavoritosEntity favorito, CancellationToken cancellationToken = default);
    void Actualizar(FavoritosEntity favorito);
    void EliminarLogico(FavoritosEntity favorito);
}
