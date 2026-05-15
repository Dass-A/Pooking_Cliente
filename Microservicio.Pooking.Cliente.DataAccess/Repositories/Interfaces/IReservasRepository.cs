using Microservicio.Pooking.Cliente.DataAccess.Common;
using Microservicio.Pooking.Cliente.DataAccess.Entities;

namespace Microservicio.Pooking.Cliente.DataAccess.Repositories.Interfaces;

/// <summary>
/// Repositorio de reservas — solo creación y lectura.
/// La transición de estados (CANC, COMP) la realiza otro microservicio (Pago/Proveedor).
/// </summary>
public interface IReservasRepository
{
    // Lecturas
    Task<ReservasEntity?> ObtenerPorIdAsync(long idReserva, CancellationToken cancellationToken = default);
    Task<ReservasEntity?> ObtenerPorGuidAsync(Guid guidReserva, CancellationToken cancellationToken = default);

    Task<PagedResult<ReservasEntity>> ObtenerPorClientePaginadoAsync(
        int idCliente,
        int paginaActual,
        int tamanioPagina,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ReservasEntity>> ObtenerPorEstadoPaginadoAsync(
        string estado,
        int paginaActual,
        int tamanioPagina,
        CancellationToken cancellationToken = default);

    // Escritura
    Task AgregarAsync(ReservasEntity reserva, CancellationToken cancellationToken = default);
}
