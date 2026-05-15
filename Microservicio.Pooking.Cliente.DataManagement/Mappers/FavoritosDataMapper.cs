using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Mappers;

public static class FavoritosDataMapper
{
    public static FavoritosDataModel ToDataModel(FavoritosEntity entity) => new()
    {
        IdFavorito = entity.IdFavorito,
        GuidFavorito = entity.GuidFavorito,
        GuidClienteRef = entity.GuidClienteRef,
        GuidServicioRef = entity.GuidServicioRef,
        Alias = entity.Alias,
        Estado = entity.Estado,
        CreadoPorUsuario = entity.CreadoPorUsuario,
        FechaRegistroUtc = entity.FechaRegistroUtc
    };

    public static FavoritosEntity ToEntity(FavoritosDataModel model) => new()
    {
        IdFavorito = model.IdFavorito,
        GuidFavorito = model.GuidFavorito,
        GuidClienteRef = model.GuidClienteRef,
        GuidServicioRef = model.GuidServicioRef,
        Alias = model.Alias,
        Estado = model.Estado,
        CreadoPorUsuario = model.CreadoPorUsuario,
        FechaRegistroUtc = model.FechaRegistroUtc
    };
}
