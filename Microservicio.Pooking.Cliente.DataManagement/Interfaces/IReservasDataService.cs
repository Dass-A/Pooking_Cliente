using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Interfaces;

public interface IReservasDataService
{
    // Lecturas
    Task<ReservasDataModel?> ObtenerPorGuidAsync(Guid guidReserva, CancellationToken cancellationToken = default);

    Task<PagedDataResult<ReservasDataModel>> ListarPorClienteAsync(
        int idCliente,
        int pagina,
        int tamanio,
        CancellationToken cancellationToken = default);

    Task<PagedDataResult<ReservasDataModel>> ListarPorEstadoAsync(
        string estado,
        int pagina,
        int tamanio,
        CancellationToken cancellationToken = default);

    // Escritura — solo crear; las transiciones de estado las maneja otro microservicio
    Task<ReservasDataModel> CrearAsync(ReservasDataModel reserva, CancellationToken cancellationToken = default);
}
