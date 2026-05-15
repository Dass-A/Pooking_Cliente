using Microservicio.Pooking.Cliente.DataAccess.Common;
using Microservicio.Pooking.Cliente.DataAccess.Context;
using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microservicio.Pooking.Cliente.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Cliente.DataAccess.Repositories;

public class ReservasRepository : IReservasRepository
{
    private readonly ClienteDbContext _context;

    public ReservasRepository(ClienteDbContext context)
    {
        _context = context;
    }

    private IQueryable<ReservasEntity> QueryVigentes =>
        _context.Reservas.Where(r => !r.EsEliminado);

    // -------------------------------------------------------------------------
    // Lecturas
    // -------------------------------------------------------------------------

    public async Task<ReservasEntity?> ObtenerPorIdAsync(long idReserva, CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(r => r.IdReserva == idReserva, cancellationToken);

    public async Task<ReservasEntity?> ObtenerPorGuidAsync(Guid guidReserva, CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(r => r.GuidReserva == guidReserva, cancellationToken);

    public async Task<PagedResult<ReservasEntity>> ObtenerPorClientePaginadoAsync(
        int idCliente,
        int paginaActual,
        int tamanioPagina,
        CancellationToken cancellationToken = default)
    {
        var query = QueryVigentes
            .Where(r => r.IdCliente == idCliente)
            .OrderByDescending(r => r.FechaReservaUtc);

        var total = await query.CountAsync(cancellationToken);

        if (total == 0)
            return PagedResult<ReservasEntity>.Vacio(paginaActual, tamanioPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<ReservasEntity>(items, total, paginaActual, tamanioPagina);
    }

    public async Task<PagedResult<ReservasEntity>> ObtenerPorEstadoPaginadoAsync(
        string estado,
        int paginaActual,
        int tamanioPagina,
        CancellationToken cancellationToken = default)
    {
        var query = QueryVigentes
            .Where(r => r.Estado == estado)
            .OrderByDescending(r => r.FechaReservaUtc);

        var total = await query.CountAsync(cancellationToken);

        if (total == 0)
            return PagedResult<ReservasEntity>.Vacio(paginaActual, tamanioPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<ReservasEntity>(items, total, paginaActual, tamanioPagina);
    }

    // -------------------------------------------------------------------------
    // Escritura
    // -------------------------------------------------------------------------

    public async Task AgregarAsync(ReservasEntity reserva, CancellationToken cancellationToken = default)
    {
        await _context.Reservas.AddAsync(reserva, cancellationToken);
    }
}
