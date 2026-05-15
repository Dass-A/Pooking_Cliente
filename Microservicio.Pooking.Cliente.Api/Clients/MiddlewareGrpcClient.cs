using Booking.Middleware.Api.Protos;
using Grpc.Core;
using Microservicio.Pooking.Cliente.Business.Clients;

namespace Microservicio.Pooking.Cliente.Api.Clients;

/// <summary>
/// Implementación del cliente gRPC hacia el Booking.Middleware.
///
/// Envuelve los dos servicios generados desde eventos.proto:
///   - ResolverGrpcServiceClient   (síncrono — resolver datos cross-dominio)
///   - EventBusGrpcServiceClient   (asíncrono — publicar eventos de auditoría)
///
/// Política de errores:
///   - Resolver: si falla, relanza. El negocio depende de la respuesta.
///   - EventBus: si falla, loggea pero NO relanza. La publicación de eventos es
///     fire-and-forget en términos de negocio; el evento puede recuperarse
///     después vía outbox pattern (futuro).
/// </summary>
public class MiddlewareGrpcClient : IMiddlewareGrpcClient
{
    private readonly ResolverGrpcService.ResolverGrpcServiceClient _resolverClient;
    private readonly EventBusGrpcService.EventBusGrpcServiceClient _eventBusClient;
    private readonly ILogger<MiddlewareGrpcClient> _logger;

    public MiddlewareGrpcClient(
        ResolverGrpcService.ResolverGrpcServiceClient resolverClient,
        EventBusGrpcService.EventBusGrpcServiceClient eventBusClient,
        ILogger<MiddlewareGrpcClient> logger)
    {
        _resolverClient = resolverClient;
        _eventBusClient = eventBusClient;
        _logger = logger;
    }

    // =========================================================================
    // Resolver — síncrono
    // =========================================================================

    public async Task<ServicioResueltoDto?> ResolverServicioAsync(
        Guid guidServicio,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var reply = await _resolverClient.ResolverServicioAsync(
                new ResolverServicioRequest { GuidServicio = guidServicio.ToString() },
                cancellationToken: cancellationToken);

            if (!reply.Found)
            {
                _logger.LogInformation(
                    "Middleware.ResolverServicio: servicio {Guid} no encontrado.",
                    guidServicio);
                return null;
            }

            return new ServicioResueltoDto
            {
                GuidServicio = Guid.Parse(reply.GuidServicio),
                RazonSocial = reply.RazonSocial,
                NombreComercial = reply.NombreComercial,
                TipoServicio = reply.TipoServicio,
                Estado = reply.Estado,
                CorreoContacto = reply.CorreoContacto,
                TelefonoContacto = reply.TelefonoContacto,
                LogoUrl = reply.LogoUrl
            };
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex,
                "Middleware.ResolverServicio: error gRPC consultando {Guid}. Status={Status}",
                guidServicio, ex.StatusCode);
            throw;
        }
    }

    public async Task<DisponibilidadResueltaDto> ValidarDisponibilidadAsync(
        Guid guidServicio,
        DateTime fechaInicio,
        DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ValidarDisponibilidadRequest
            {
                GuidServicio = guidServicio.ToString(),
                FechaInicio = fechaInicio.ToString("o"),  // ISO 8601
                FechaFin = fechaFin?.ToString("o") ?? string.Empty
            };

            var reply = await _resolverClient.ValidarDisponibilidadServicioAsync(
                request,
                cancellationToken: cancellationToken);

            return new DisponibilidadResueltaDto
            {
                Disponible = reply.Disponible,
                Motivo = reply.Motivo
            };
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex,
                "Middleware.ValidarDisponibilidad: error gRPC para servicio {Guid}. Status={Status}",
                guidServicio, ex.StatusCode);
            throw;
        }
    }

    // =========================================================================
    // EventBus — asíncrono (no relanza errores)
    // =========================================================================

    public async Task PublicarEventoAsync(
        EventoAuditoriaDto evento,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new EventoRequest
            {
                IdCorrelacion = evento.IdCorrelacion,
                TablaAfectada = evento.TablaAfectada,
                EsquemaAfectado = evento.EsquemaAfectado,
                Operacion = MapearOperacion(evento.Operacion),
                IdRegistro = evento.IdRegistro,
                DatosAnteriores = evento.DatosAnteriores ?? string.Empty,
                DatosNuevos = evento.DatosNuevos ?? string.Empty,
                Usuario = evento.Usuario ?? string.Empty,
                Ip = evento.Ip ?? string.Empty,
                ServicioOrigen = evento.ServicioOrigen,
                EquipoOrigen = evento.EquipoOrigen ?? string.Empty
            };

            var reply = await _eventBusClient.PublicarEventoAsync(
                request,
                cancellationToken: cancellationToken);

            if (!reply.Success)
            {
                _logger.LogWarning(
                    "Middleware.PublicarEvento: rechazado por el bus. IdCorrelacion={IdCorrelacion}, Mensaje={Mensaje}",
                    evento.IdCorrelacion, reply.Mensaje);
            }
        }
        catch (RpcException ex)
        {
            // NO relanzamos: la publicación de eventos no debe romper la transacción de negocio.
            _logger.LogError(ex,
                "Middleware.PublicarEvento: error gRPC publicando evento {Tabla}/{Operacion} ({IdCorrelacion}). Status={Status}",
                evento.TablaAfectada, evento.Operacion, evento.IdCorrelacion, ex.StatusCode);
        }
    }

    private static TipoOperacion MapearOperacion(TipoOperacionAuditoria op) =>
        op switch
        {
            TipoOperacionAuditoria.Insert => TipoOperacion.Insert,
            TipoOperacionAuditoria.Update => TipoOperacion.Update,
            TipoOperacionAuditoria.Delete => TipoOperacion.Delete,
            _ => TipoOperacion.Insert
        };
}
