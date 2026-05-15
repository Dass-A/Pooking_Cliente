using Microservicio.Pooking.Cliente.DataManagement.Interfaces;
using Microservicio.Pooking.Cliente.DataManagement.Mappers;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Services;

public class ClienteDataService : IClienteDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClienteDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // -------------------------------------------------------------------------
    // Lecturas
    // -------------------------------------------------------------------------

    public async Task<ClienteDataModel?> ObtenerPorIdAsync(int idCliente, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClienteRepository.ObtenerPorIdAsync(idCliente, cancellationToken);
        return entity is null ? null : ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel?> ObtenerPorGuidAsync(Guid guidCliente, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClienteRepository.ObtenerPorGuidAsync(guidCliente, cancellationToken);
        return entity is null ? null : ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel?> ObtenerPorUsuarioGuidRefAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClienteRepository.ObtenerPorUsuarioGuidRefAsync(usuarioGuidRef, cancellationToken);
        return entity is null ? null : ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClienteRepository.ObtenerPorCorreoAsync(correo, cancellationToken);
        return entity is null ? null : ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel?> ObtenerPorNumeroIdentificacionAsync(
        string tipoIdentificacion,
        string numeroIdentificacion,
        CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClienteRepository.ObtenerPorNumeroIdentificacionAsync(
            tipoIdentificacion, numeroIdentificacion, cancellationToken);
        return entity is null ? null : ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<PagedDataResult<ClienteDataModel>> ListarPaginadoAsync(
        int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        var paged = await _unitOfWork.ClienteRepository.ObtenerTodosPaginadoAsync(pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ClienteDataMapper.ToDataModel);
        return new PagedDataResult<ClienteDataModel>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<PagedDataResult<ClienteDataModel>> ListarPorEstadoAsync(
        string estado, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        var paged = await _unitOfWork.ClienteRepository.ObtenerPorEstadoPaginadoAsync(estado, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ClienteDataMapper.ToDataModel);
        return new PagedDataResult<ClienteDataModel>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    // -------------------------------------------------------------------------
    // Verificaciones
    // -------------------------------------------------------------------------

    public async Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken = default) =>
        await _unitOfWork.ClienteRepository.ExisteCorreoAsync(correo, cancellationToken);

    public async Task<bool> ExisteNumeroIdentificacionAsync(
        string tipoIdentificacion, string numeroIdentificacion, CancellationToken cancellationToken = default) =>
        await _unitOfWork.ClienteRepository.ExisteNumeroIdentificacionAsync(tipoIdentificacion, numeroIdentificacion, cancellationToken);

    public async Task<bool> ExisteUsuarioVinculadoAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default) =>
        await _unitOfWork.ClienteRepository.ExisteUsuarioVinculadoAsync(usuarioGuidRef, cancellationToken);

    // -------------------------------------------------------------------------
    // Escritura
    // -------------------------------------------------------------------------

    public async Task<ClienteDataModel> CrearAsync(ClienteDataModel cliente, CancellationToken cancellationToken = default)
    {
        var entity = ClienteDataMapper.ToEntity(cliente);
        entity.FechaRegistroUtc = DateTimeOffset.UtcNow;
        entity.Estado = "ACT";
        entity.EsEliminado = false;

        await _unitOfWork.ClienteRepository.AgregarAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ClienteDataMapper.ToDataModel(entity);
    }

    public async Task<ClienteDataModel> ActualizarAsync(ClienteDataModel cliente, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClienteRepository.ObtenerPorGuidAsync(cliente.GuidCliente, cancellationToken)
            ?? throw new InvalidOperationException($"Cliente {cliente.GuidCliente} no encontrado.");

        ClienteDataMapper.UpdateEntity(entity, cliente);
        _unitOfWork.ClienteRepository.Actualizar(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ClienteDataMapper.ToDataModel(entity);
    }

    public async Task EliminarLogicoAsync(Guid guidCliente, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClienteRepository.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new InvalidOperationException($"Cliente {guidCliente} no encontrado.");

        _unitOfWork.ClienteRepository.EliminarLogico(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CambiarEstadoAsync(Guid guidCliente, string nuevoEstado, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ClienteRepository.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new InvalidOperationException($"Cliente {guidCliente} no encontrado.");

        _unitOfWork.ClienteRepository.CambiarEstado(entity, nuevoEstado);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
