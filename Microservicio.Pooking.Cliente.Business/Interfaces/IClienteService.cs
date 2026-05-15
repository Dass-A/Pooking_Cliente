using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Cliente;

namespace Microservicio.Pooking.Cliente.Business.Interfaces;

public interface IClienteService
{
    // Consultas
    Task<ClienteResponse> ObtenerPorGuidAsync(Guid guidCliente, CancellationToken cancellationToken = default);
    Task<ClienteResponse> ObtenerPorUsuarioGuidRefAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default);
    Task<ClienteResponse> ObtenerPorNumeroIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, CancellationToken cancellationToken = default);
    Task<PagedResultado<ClienteResponse>> ListarPaginadoAsync(int pagina, int tamanio, CancellationToken cancellationToken = default);
    Task<PagedResultado<ClienteResponse>> ListarPorEstadoAsync(string estado, int pagina, int tamanio, CancellationToken cancellationToken = default);

    // Comandos
    Task<ClienteResponse> CrearAsync(CrearClienteRequest request, CancellationToken cancellationToken = default);
    Task<ClienteResponse> ActualizarAsync(Guid guidCliente, ActualizarClienteRequest request, CancellationToken cancellationToken = default);
    Task EliminarLogicoAsync(Guid guidCliente, CancellationToken cancellationToken = default);
    Task CambiarEstadoAsync(Guid guidCliente, string nuevoEstado, CancellationToken cancellationToken = default);
}
