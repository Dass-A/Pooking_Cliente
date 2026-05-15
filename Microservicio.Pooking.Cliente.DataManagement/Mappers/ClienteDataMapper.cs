using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Mappers;

public static class ClienteDataMapper
{
    public static ClienteDataModel ToDataModel(ClienteEntity entity) => new()
    {
        IdCliente = entity.IdCliente,
        GuidCliente = entity.GuidCliente,
        UsuarioGuidRef = entity.UsuarioGuidRef,
        Nombres = entity.Nombres,
        Apellidos = entity.Apellidos,
        RazonSocial = entity.RazonSocial,
        TipoIdentificacion = entity.TipoIdentificacion,
        NumeroIdentificacion = entity.NumeroIdentificacion,
        Correo = entity.Correo,
        Telefono = entity.Telefono,
        Direccion = entity.Direccion,
        Estado = entity.Estado,
        EsEliminado = entity.EsEliminado,
        CreadoPorUsuario = entity.CreadoPorUsuario,
        FechaRegistroUtc = entity.FechaRegistroUtc,
        ModificadoPorUsuario = entity.ModificadoPorUsuario,
        FechaModificacionUtc = entity.FechaModificacionUtc
    };

    public static ClienteEntity ToEntity(ClienteDataModel model) => new()
    {
        IdCliente = model.IdCliente,
        GuidCliente = model.GuidCliente,
        UsuarioGuidRef = model.UsuarioGuidRef,
        Nombres = model.Nombres,
        Apellidos = model.Apellidos,
        RazonSocial = model.RazonSocial,
        TipoIdentificacion = model.TipoIdentificacion,
        NumeroIdentificacion = model.NumeroIdentificacion,
        Correo = model.Correo,
        Telefono = model.Telefono,
        Direccion = model.Direccion,
        Estado = model.Estado,
        EsEliminado = model.EsEliminado,
        CreadoPorUsuario = model.CreadoPorUsuario,
        FechaRegistroUtc = model.FechaRegistroUtc,
        ModificadoPorUsuario = model.ModificadoPorUsuario,
        FechaModificacionUtc = model.FechaModificacionUtc
    };

    public static void UpdateEntity(ClienteEntity entity, ClienteDataModel model)
    {
        entity.Nombres = model.Nombres;
        entity.Apellidos = model.Apellidos;
        entity.RazonSocial = model.RazonSocial;
        entity.TipoIdentificacion = model.TipoIdentificacion;
        entity.NumeroIdentificacion = model.NumeroIdentificacion;
        entity.Correo = model.Correo;
        entity.Telefono = model.Telefono;
        entity.Direccion = model.Direccion;
        entity.Estado = model.Estado;
        entity.ModificadoPorUsuario = model.ModificadoPorUsuario;
        entity.FechaModificacionUtc = DateTimeOffset.UtcNow;
    }
}
