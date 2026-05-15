using Microservicio.Pooking.Cliente.DataAccess.Repositories.Interfaces;

namespace Microservicio.Pooking.Cliente.DataManagement.Interfaces;

/// <summary>
/// Unidad de trabajo del microservicio Cliente.
/// Agrupa los 3 repositorios del dominio (cliente, reservas, favoritos)
/// bajo un único DbContext y maneja el SaveChangesAsync de la transacción lógica.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IClienteRepository ClienteRepository { get; }
    IReservasRepository ReservasRepository { get; }
    IFavoritosRepository FavoritosRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
