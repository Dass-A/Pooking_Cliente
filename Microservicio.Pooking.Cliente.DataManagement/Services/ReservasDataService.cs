using Microservicio.Pooking.Cliente.DataManagement.Interfaces;
using Microservicio.Pooking.Cliente.DataManagement.Mappers;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Services;

public class ReservasDataService : IReservasDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservasDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservasDataModel?> ObtenerPorGuidAsync(Guid guidReserva, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ReservasRepository.ObtenerPorGuidAsync(guidReserva, cancellationToken);
        return entity is null ? null : ReservasDataMapper.ToDataModel(entity);
    }

    public async Task<PagedDataResult<ReservasDataModel>> ListarPorClienteAsync(
        int idCliente, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        var paged = await _unitOfWork.ReservasRepository.ObtenerPorClientePaginadoAsync(
            idCliente, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ReservasDataMapper.ToDataModel);
        return new PagedDataResult<ReservasDataModel>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<PagedDataResult<ReservasDataModel>> ListarPorEstadoAsync(
        string estado, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        var paged = await _unitOfWork.ReservasRepository.ObtenerPorEstadoPaginadoAsync(
            estado, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ReservasDataMapper.ToDataModel);
        return new PagedDataResult<ReservasDataModel>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<ReservasDataModel> CrearAsync(ReservasDataModel reserva, CancellationToken cancellationToken = default)
    {
        var entity = ReservasDataMapper.ToEntity(reserva);
        entity.FechaRegistroUtc = DateTimeOffset.UtcNow;
        entity.FechaReservaUtc = DateTimeOffset.UtcNow;
        // Estado inicial CONF: este microservicio solo crea reservas.
        // Si otro microservicio (Pago/Proveedor) las cancela o completa, lo hará por su lado.
        entity.Estado = "CONF";
        entity.EsEliminado = false;

        await _unitOfWork.ReservasRepository.AgregarAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ReservasDataMapper.ToDataModel(entity);
    }
}
