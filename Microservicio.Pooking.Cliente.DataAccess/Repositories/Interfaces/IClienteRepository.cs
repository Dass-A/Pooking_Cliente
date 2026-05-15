using Microservicio.Pooking.Cliente.DataAccess.Common;
using Microservicio.Pooking.Cliente.DataAccess.Entities;

namespace Microservicio.Pooking.Cliente.DataAccess.Repositories.Interfaces;

/// <summary>
/// Contrato del repositorio de clientes.
/// Ningún método llama SaveChanges directamente — esa responsabilidad recae en la UoW.
/// </summary>
public interface IClienteRepository
{
    // Lecturas simples
    Task<ClienteEntity?> ObtenerPorIdAsync(int idCliente, CancellationToken cancellationToken = default);
    Task<ClienteEntity?> ObtenerPorGuidAsync(Guid guidCliente, CancellationToken cancellationToken = default);
    Task<ClienteEntity?> ObtenerPorUsuarioGuidRefAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default);
    Task<ClienteEntity?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task<ClienteEntity?> ObtenerPorNumeroIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, CancellationToken cancellationToken = default);

    // Lecturas paginadas
    Task<PagedResult<ClienteEntity>> ObtenerTodosPaginadoAsync(int paginaActual, int tamanioPagina, CancellationToken cancellationToken = default);
    Task<PagedResult<ClienteEntity>> ObtenerPorEstadoPaginadoAsync(string estado, int paginaActual, int tamanioPagina, CancellationToken cancellationToken = default);

    // Verificaciones
    Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken = default);
    Task<bool> ExisteNumeroIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion, CancellationToken cancellationToken = default);
    Task<bool> ExisteUsuarioVinculadoAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default);

    // Escritura
    Task AgregarAsync(ClienteEntity cliente, CancellationToken cancellationToken = default);
    void Actualizar(ClienteEntity cliente);
    void EliminarLogico(ClienteEntity cliente);
    void CambiarEstado(ClienteEntity cliente, string nuevoEstado);
}
