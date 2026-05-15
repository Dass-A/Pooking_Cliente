using Microservicio.Pooking.Cliente.Business.DTOs.Favoritos;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.Business.Mappers;

public static class FavoritosBusinessMapper
{
    public static FavoritoResponse ToResponse(FavoritosDataModel model) => new()
    {
        GuidFavorito = model.GuidFavorito,
        GuidClienteRef = model.GuidClienteRef,
        GuidServicioRef = model.GuidServicioRef,
        Alias = model.Alias,
        Estado = model.Estado,
        FechaRegistroUtc = model.FechaRegistroUtc
    };

    public static FavoritosDataModel ToDataModelFromCreate(CrearFavoritoRequest request) => new()
    {
        GuidClienteRef = request.GuidClienteRef,
        GuidServicioRef = request.GuidServicioRef,
        Alias = request.Alias
    };
}
