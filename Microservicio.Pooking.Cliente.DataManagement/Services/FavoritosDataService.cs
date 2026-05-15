using Microservicio.Pooking.Cliente.DataManagement.Interfaces;
using Microservicio.Pooking.Cliente.DataManagement.Mappers;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Services;

public class FavoritosDataService : IFavoritosDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public FavoritosDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FavoritosDataModel?> ObtenerPorGuidAsync(Guid guidFavorito, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FavoritosRepository.ObtenerPorGuidAsync(guidFavorito, cancellationToken);
        return entity is null ? null : FavoritosDataMapper.ToDataModel(entity);
    }

    public async Task<FavoritosDataModel?> ObtenerPorClienteYServicioAsync(
        Guid guidClienteRef, Guid guidServicioRef, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FavoritosRepository.ObtenerPorClienteYServicioAsync(
            guidClienteRef, guidServicioRef, cancellationToken);
        return entity is null ? null : FavoritosDataMapper.ToDataModel(entity);
    }

    public async Task<PagedDataResult<FavoritosDataModel>> ListarPorClienteAsync(
        Guid guidClienteRef, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        var paged = await _unitOfWork.FavoritosRepository.ObtenerPorClientePaginadoAsync(
            guidClienteRef, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(FavoritosDataMapper.ToDataModel);
        return new PagedDataResult<FavoritosDataModel>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<bool> ExisteFavoritoAsync(
        Guid guidClienteRef, Guid guidServicioRef, CancellationToken cancellationToken = default) =>
        await _unitOfWork.FavoritosRepository.ExisteFavoritoAsync(guidClienteRef, guidServicioRef, cancellationToken);

    public async Task<FavoritosDataModel> CrearAsync(FavoritosDataModel favorito, CancellationToken cancellationToken = default)
    {
        var entity = FavoritosDataMapper.ToEntity(favorito);
        entity.FechaRegistroUtc = DateTimeOffset.UtcNow;
        entity.Estado = "ACT";
        entity.EsEliminado = false;

        await _unitOfWork.FavoritosRepository.AgregarAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return FavoritosDataMapper.ToDataModel(entity);
    }

    public async Task<FavoritosDataModel> ActualizarAsync(FavoritosDataModel favorito, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FavoritosRepository.ObtenerPorGuidAsync(favorito.GuidFavorito, cancellationToken)
            ?? throw new InvalidOperationException($"Favorito {favorito.GuidFavorito} no encontrado.");

        entity.Alias = favorito.Alias;
        entity.Estado = favorito.Estado;
        entity.FechaModificacionUtc = DateTimeOffset.UtcNow;

        _unitOfWork.FavoritosRepository.Actualizar(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return FavoritosDataMapper.ToDataModel(entity);
    }

    public async Task EliminarLogicoAsync(Guid guidFavorito, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FavoritosRepository.ObtenerPorGuidAsync(guidFavorito, cancellationToken)
            ?? throw new InvalidOperationException($"Favorito {guidFavorito} no encontrado.");

        _unitOfWork.FavoritosRepository.EliminarLogico(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
