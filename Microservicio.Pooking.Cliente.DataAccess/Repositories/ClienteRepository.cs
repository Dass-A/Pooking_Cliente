using Microservicio.Pooking.Cliente.DataAccess.Common;
using Microservicio.Pooking.Cliente.DataAccess.Context;
using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microservicio.Pooking.Cliente.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Cliente.DataAccess.Repositories;

/// <summary>
/// Implementación de IClienteRepository.
/// Toda operación de escritura requiere que la UoW invoque SaveChangesAsync.
/// </summary>
public class ClienteRepository : IClienteRepository
{
    private readonly ClienteDbContext _context;

    public ClienteRepository(ClienteDbContext context)
    {
        _context = context;
    }

    private IQueryable<ClienteEntity> QueryVigentes =>
        _context.Clientes.Where(c => !c.EsEliminado);

    // -------------------------------------------------------------------------
    // Lecturas
    // -------------------------------------------------------------------------

    public async Task<ClienteEntity?> ObtenerPorIdAsync(int idCliente, CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(c => c.IdCliente == idCliente, cancellationToken);

    public async Task<ClienteEntity?> ObtenerPorGuidAsync(Guid guidCliente, CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(c => c.GuidCliente == guidCliente, cancellationToken);

    public async Task<ClienteEntity?> ObtenerPorUsuarioGuidRefAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(c => c.UsuarioGuidRef == usuarioGuidRef, cancellationToken);

    public async Task<ClienteEntity?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(c => c.Correo == correo, cancellationToken);

    public async Task<ClienteEntity?> ObtenerPorNumeroIdentificacionAsync(
        string tipoIdentificacion,
        string numeroIdentificacion,
        CancellationToken cancellationToken = default) =>
        await QueryVigentes.FirstOrDefaultAsync(
            c => c.TipoIdentificacion == tipoIdentificacion && c.NumeroIdentificacion == numeroIdentificacion,
            cancellationToken);

    public async Task<PagedResult<ClienteEntity>> ObtenerTodosPaginadoAsync(
        int paginaActual,
        int tamanioPagina,
        CancellationToken cancellationToken = default)
    {
        var query = QueryVigentes.OrderBy(c => c.IdCliente);
        var total = await query.CountAsync(cancellationToken);

        if (total == 0)
            return PagedResult<ClienteEntity>.Vacio(paginaActual, tamanioPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<ClienteEntity>(items, total, paginaActual, tamanioPagina);
    }

    public async Task<PagedResult<ClienteEntity>> ObtenerPorEstadoPaginadoAsync(
        string estado,
        int paginaActual,
        int tamanioPagina,
        CancellationToken cancellationToken = default)
    {
        var query = QueryVigentes.Where(c => c.Estado == estado).OrderBy(c => c.IdCliente);
        var total = await query.CountAsync(cancellationToken);

        if (total == 0)
            return PagedResult<ClienteEntity>.Vacio(paginaActual, tamanioPagina);

        var items = await query
            .Skip((paginaActual - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<ClienteEntity>(items, total, paginaActual, tamanioPagina);
    }

    // -------------------------------------------------------------------------
    // Verificaciones
    // -------------------------------------------------------------------------

    public async Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken = default) =>
        await _context.Clientes.AnyAsync(c => c.Correo == correo, cancellationToken);

    public async Task<bool> ExisteNumeroIdentificacionAsync(
        string tipoIdentificacion,
        string numeroIdentificacion,
        CancellationToken cancellationToken = default) =>
        await _context.Clientes.AnyAsync(
            c => c.TipoIdentificacion == tipoIdentificacion && c.NumeroIdentificacion == numeroIdentificacion,
            cancellationToken);

    public async Task<bool> ExisteUsuarioVinculadoAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default) =>
        await _context.Clientes.AnyAsync(c => c.UsuarioGuidRef == usuarioGuidRef, cancellationToken);

    // -------------------------------------------------------------------------
    // Escritura
    // -------------------------------------------------------------------------

    public async Task AgregarAsync(ClienteEntity cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
    }

    public void Actualizar(ClienteEntity cliente)
    {
        _context.Clientes.Update(cliente);
    }

    public void EliminarLogico(ClienteEntity cliente)
    {
        cliente.EsEliminado = true;
        cliente.Estado = "INA";
        _context.Clientes.Update(cliente);
    }

    public void CambiarEstado(ClienteEntity cliente, string nuevoEstado)
    {
        cliente.Estado = nuevoEstado;
        _context.Clientes.Update(cliente);
    }
}
