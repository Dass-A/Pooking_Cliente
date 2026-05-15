using Microservicio.Pooking.Cliente.DataAccess.Common;
using Microservicio.Pooking.Cliente.DataAccess.Context;
using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microservicio.Pooking.Cliente.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Cliente.DataAccess.Repositories;

public class FavoritosRepository : IFavoritosRepository
{
    private readonly ClienteDbContext _context;

    public FavoritosRepository(ClienteDbContext context)
    {
        _context = context;
    }

    private IQueryable<FavoritosEntity> QueryVigentes =>
        _context.Favoritos.Where(f => !f.EsEliminado);

    // -------------------------------------------------------------------------
    // Lecturas
    // -------------------------------------------------------------------------

    public async Task<FavoritosEntity?> ObtenerPorGuidAsync(Guid guidFavorito, CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(f => f.GuidFavorito == guidFavorito, cancellationToken);

    public async Task<FavoritosEntity?> ObtenerPorClienteYServicioAsync(
        Guid guidClienteRef,
        Guid guidServicioRef,
        CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(
            f => f.GuidClienteRef == guidClienteRef && f.GuidServicioRef == guidServicioRef,
            cancellationToken);

    public async Task<PagedResult<FavoritosEntity>> ObtenerPorClientePaginadoAsync(
        Guid guidClienteRef,
        int paginaActual,
        int tamanioPagina,
        CancellationToken cancellationToken = default)
    {
        var query = QueryVigentes
            .Where(f => f.GuidClienteRef == guidClienteRef)
            .OrderByDescending(f => f.FechaRegistroUtc);

        var total = await query.CountAsync(cancellationToken);

        if (total == 0)
            return PagedResult<FavoritosEntity>.Vacio(paginaActual, tamanioPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<FavoritosEntity>(items, total, paginaActual, tamanioPagina);
    }

    // -------------------------------------------------------------------------
    // Verificaciones
    // -------------------------------------------------------------------------

    public async Task<bool> ExisteFavoritoAsync(
        Guid guidClienteRef,
        Guid guidServicioRef,
        CancellationToken cancellationToken = default) =>
        await _context.Favoritos.AnyAsync(
            f => f.GuidClienteRef == guidClienteRef && f.GuidServicioRef == guidServicioRef,
            cancellationToken);

    // -------------------------------------------------------------------------
    // Escritura
    // -------------------------------------------------------------------------

    public async Task AgregarAsync(FavoritosEntity favorito, CancellationToken cancellationToken = default)
    {
        await _context.Favoritos.AddAsync(favorito, cancellationToken);
    }

    public void Actualizar(FavoritosEntity favorito)
    {
        _context.Favoritos.Update(favorito);
    }

    public void EliminarLogico(FavoritosEntity favorito)
    {
        favorito.EsEliminado = true;
        favorito.Estado = "INA";
        _context.Favoritos.Update(favorito);
    }
}
