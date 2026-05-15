using Microservicio.Pooking.Cliente.Business.DTOs.Reservas;
using Microservicio.Pooking.Cliente.DataManagement.Models;

namespace Microservicio.Pooking.Cliente.Business.Mappers;

public static class ReservasBusinessMapper
{
    public static ReservaResponse ToResponse(ReservasDataModel model) => new()
    {
        GuidReserva = model.GuidReserva,
        GuidClienteRef = model.GuidClienteRef,
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
        FechaCancelacionUtc = model.FechaCancelacionUtc
    };

    public static ReservasDataModel ToDataModelFromCreate(CrearReservaRequest request, int idCliente) => new()
    {
        IdCliente = idCliente,
        GuidClienteRef = request.GuidCliente,
        GuidServicioRef = request.GuidServicioRef,
        NombreServicioSnap = request.NombreServicioSnap,
        TipoServicioSnap = request.TipoServicioSnap,
        NombreProveedor = request.NombreProveedor,
        IdReservaExterna = request.IdReservaExterna,
        FechaInicio = request.FechaInicio,
        FechaFin = request.FechaFin,
        CanalOrigen = request.CanalOrigen,
        MontoTotal = request.MontoTotal,
        Moneda = request.Moneda,
        Observaciones = request.Observaciones,
        Estado = "CONF"
    };
}
