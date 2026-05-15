using Microservicio.Pooking.Cliente.Business.DTOs.Cliente;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.Business.Mappers;

public static class ClienteBusinessMapper
{
    public static ClienteResponse ToResponse(ClienteDataModel model) => new()
    {
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
        FechaRegistroUtc = model.FechaRegistroUtc,
        FechaModificacionUtc = model.FechaModificacionUtc
    };

    public static ClienteDataModel ToDataModelFromCreate(CrearClienteRequest request) => new()
    {
        UsuarioGuidRef = request.UsuarioGuidRef,
        Nombres = request.Nombres,
        Apellidos = request.Apellidos,
        RazonSocial = request.RazonSocial,
        TipoIdentificacion = request.TipoIdentificacion,
        NumeroIdentificacion = request.NumeroIdentificacion,
        Correo = request.Correo,
        Telefono = request.Telefono,
        Direccion = request.Direccion
    };

    public static ClienteDataModel ToDataModelFromUpdate(
        Guid guidCliente,
        ActualizarClienteRequest request,
        ClienteDataModel existente) => new()
    {
        IdCliente = existente.IdCliente,
        GuidCliente = guidCliente,
        UsuarioGuidRef = existente.UsuarioGuidRef,
        Nombres = request.Nombres,
        Apellidos = request.Apellidos,
        RazonSocial = request.RazonSocial,
        TipoIdentificacion = request.TipoIdentificacion,
        NumeroIdentificacion = request.NumeroIdentificacion,
        Correo = request.Correo,
        Telefono = request.Telefono,
        Direccion = request.Direccion,
        Estado = request.Estado,
        CreadoPorUsuario = existente.CreadoPorUsuario,
        FechaRegistroUtc = existente.FechaRegistroUtc
    };
}
