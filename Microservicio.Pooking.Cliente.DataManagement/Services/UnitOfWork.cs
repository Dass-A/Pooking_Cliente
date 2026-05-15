using Microservicio.Pooking.Cliente.DataAccess.Context;
using Microservicio.Pooking.Cliente.DataAccess.Repositories;
using Microservicio.Pooking.Cliente.DataAccess.Repositories.Interfaces;
using Microservicio.Pooking.Cliente.DataManagement.Interfaces;

namespace Microservicio.Pooking.Cliente.DataManagement.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ClienteDbContext _context;

    public IClienteRepository ClienteRepository { get; }
    public IReservasRepository ReservasRepository { get; }
    public IFavoritosRepository FavoritosRepository { get; }

    public UnitOfWork(ClienteDbContext context)
    {
        _context = context;
        ClienteRepository = new ClienteRepository(_context);
        ReservasRepository = new ReservasRepository(_context);
        FavoritosRepository = new FavoritosRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
