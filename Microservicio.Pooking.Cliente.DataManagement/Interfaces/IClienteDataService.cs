using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Interfaces;

public interface IClienteDataService
{
    // Lecturas
    Task<ClienteDataModel?> ObtenerPorIdAsync(int idCliente, CancellationToken cancellationToken = default);
    Task<ClienteDataModel?> ObtenerPorGuidAsync(Guid guidCliente, CancellationToken cancellationToken = default);
    Task<ClienteDataModel?> ObtenerPorUsuarioGuidRefAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default);
    Task<ClienteDataModel?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task<ClienteDataModel?> ObtenerPorNumeroIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, CancellationToken cancellationToken = default);

    Task<PagedDataResult<ClienteDataModel>> ListarPaginadoAsync(int pagina, int tamanio, CancellationToken cancellationToken = default);
    Task<PagedDataResult<ClienteDataModel>> ListarPorEstadoAsync(string estado, int pagina, int tamanio, CancellationToken cancellationToken = default);

    // Verificaciones
    Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task<bool> ExisteNumeroIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, CancellationToken cancellationToken = default);
    Task<bool> ExisteUsuarioVinculadoAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default);

    // Escritura
    Task<ClienteDataModel> CrearAsync(ClienteDataModel cliente, CancellationToken cancellationToken = default);
    Task<ClienteDataModel> ActualizarAsync(ClienteDataModel cliente, CancellationToken cancellationToken = default);
    Task EliminarLogicoAsync(Guid guidCliente, CancellationToken cancellationToken = default);
    Task CambiarEstadoAsync(Guid guidCliente, string nuevoEstado, CancellationToken cancellationToken = default);
}
