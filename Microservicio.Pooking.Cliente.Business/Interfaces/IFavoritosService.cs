using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Favoritos;

namespace Microservicio.Pooking.Cliente.Business.Interfaces;

public interface IFavoritosService
{
    Task<FavoritoResponse> ObtenerPorGuidAsync(Guid guidFavorito, CancellationToken cancellationToken = default);
    Task<PagedResultado<FavoritoResponse>> ListarPorClienteAsync(Guid guidCliente, int pagina, int tamanio, CancellationToken cancellationToken = default);

    Task<FavoritoResponse> CrearAsync(CrearFavoritoRequest request, CancellationToken cancellationToken = default);
    Task EliminarAsync(Guid guidFavorito, CancellationToken cancellationToken = default);
}
