using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Reservas;

namespace Microservicio.Pooking.Cliente.Business.Interfaces;

/// <summary>
/// Servicio de reservas — solo creación y consulta.
/// Las transiciones de estado (CANC, COMP) las realiza otro microservicio.
/// </summary>
public interface IReservasService
{
    Task<ReservaResponse> ObtenerPorGuidAsync(Guid guidReserva, CancellationToken cancellationToken = default);
    Task<PagedResultado<ReservaResponse>> ListarPorClienteAsync(Guid guidCliente, int pagina, int tamanio, CancellationToken cancellationToken = default);
    Task<PagedResultado<ReservaResponse>> ListarPorEstadoAsync(string estado, int pagina, int tamanio, CancellationToken cancellationToken = default);

    Task<ReservaResponse> CrearAsync(CrearReservaRequest request, CancellationToken cancellationToken = default);
}
