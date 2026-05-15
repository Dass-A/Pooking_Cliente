using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.DataManagement.Mappers;

public static class ReservasDataMapper
{
    public static ReservasDataModel ToDataModel(ReservasEntity entity) => new()
    {
        IdReserva = entity.IdReserva,
        GuidReserva = entity.GuidReserva,
        IdCliente = entity.IdCliente,
        GuidClienteRef = entity.Cliente?.GuidCliente ?? Guid.Empty,
        GuidServicioRef = entity.GuidServicioRef,
        NombreServicioSnap = entity.NombreServicioSnap,
        TipoServicioSnap = entity.TipoServicioSnap,
        NombreProveedor = entity.NombreProveedor,
        IdReservaExterna = entity.IdReservaExterna,
        FechaReservaUtc = entity.FechaReservaUtc,
        FechaInicio = entity.FechaInicio,
        FechaFin = entity.FechaFin,
        CanalOrigen = entity.CanalOrigen,
        MontoTotal = entity.MontoTotal,
        Moneda = entity.Moneda,
        Observaciones = entity.Observaciones,
        Estado = entity.Estado,
        MotivoCancelacion = entity.MotivoCancelacion,
        FechaCancelacionUtc = entity.FechaCancelacionUtc,
        CreadoPorUsuario = entity.CreadoPorUsuario,
        FechaRegistroUtc = entity.FechaRegistroUtc
    };

    public static ReservasEntity ToEntity(ReservasDataModel model) => new()
    {
        IdReserva = model.IdReserva,
        GuidReserva = model.GuidReserva,
        IdCliente = model.IdCliente,
        GuidServicioRef = model.GuidServicioRef,
        NombreServicioSnap = model.NombreServicioSnap,
        TipoServicioSnap = model.TipoServicioSnap,
        NombreProveedor = model.NombreProveedor,
        IdReservaExterna = model.IdReservaExterna,
        FechaReservaUtc = model.FechaReservaUtc,
        FechaInicio = model.FechaInicio,
        FechaFin = model.FechaFin,
        CanalOrigen = model.CanalOrigen,
        MontoTotal = model.MontoTotal,
        Moneda = model.Moneda,
        Observaciones = model.Observaciones,
        Estado = model.Estado,
        MotivoCancelacion = model.MotivoCancelacion,
        FechaCancelacionUtc = model.FechaCancelacionUtc,
        CreadoPorUsuario = model.CreadoPorUsuario,
        FechaRegistroUtc = model.FechaRegistroUtc
    };
}
