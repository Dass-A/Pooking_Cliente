using Booking.Middleware.Api.Protos;
using Microservicio.Pooking.Cliente.Api.Clients;
using Microservicio.Pooking.Cliente.Api.Models.Settings;
using Microservicio.Pooking.Cliente.Business.Clients;

namespace Microservicio.Pooking.Cliente.Api.Extensions;

/// <summary>
/// Registra el cliente gRPC hacia el Booking.Middleware.
///
/// Único cliente gRPC que este microservicio consume (regla del plan v2.0).
/// La URL se configura en appsettings.json bajo GrpcEndpoints:Middleware.
/// </summary>
public static class GrpcClientsExtensions
{
    public static IServiceCollection AddMiddlewareGrpcClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetSection(GrpcEndpointsSettings.SectionName)
            .Get<GrpcEndpointsSettings>() ?? new GrpcEndpointsSettings();

        services.Configure<GrpcEndpointsSettings>(
            configuration.GetSection(GrpcEndpointsSettings.SectionName));

        if (settings.Enabled && !string.IsNullOrWhiteSpace(settings.Middleware))
        {
            // Cliente gRPC habilitado → registra los dos stubs generados de eventos.proto
            services.AddGrpcClient<ResolverGrpcService.ResolverGrpcServiceClient>(options =>
            {
                options.Address = new Uri(settings.Middleware);
            });

            services.AddGrpcClient<EventBusGrpcService.EventBusGrpcServiceClient>(options =>
            {
                options.Address = new Uri(settings.Middleware);
            });

            services.AddScoped<IMiddlewareGrpcClient, MiddlewareGrpcClient>();
        }
        else
        {
            // Modo dev sin Middleware: stub no-op para que el resto del código
            // pueda depender de IMiddlewareGrpcClient sin ifs.
            services.AddScoped<IMiddlewareGrpcClient, MiddlewareGrpcClientDisabled>();
        }

        return services;
    }
}

/// <summary>
/// Stub no-op para cuando el Middleware no está disponible (desarrollo local).
/// - ResolverServicio devuelve null (tratado como "no encontrado").
/// - ValidarDisponibilidad devuelve disponible=true (asume optimista en dev).
/// - PublicarEvento descarta silenciosamente.
///
/// Cuando el Middleware esté disponible, configurar GrpcEndpoints:Enabled = true.
/// </summary>
internal class MiddlewareGrpcClientDisabled : IMiddlewareGrpcClient
{
    private readonly ILogger<MiddlewareGrpcClientDisabled> _logger;

    public MiddlewareGrpcClientDisabled(ILogger<MiddlewareGrpcClientDisabled> logger)
    {
        _logger = logger;
    }

    public Task<ServicioResueltoDto?> ResolverServicioAsync(
        Guid guidServicio, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Middleware deshabilitado: ResolverServicio({Guid}) → null (modo dev).",
            guidServicio);
        return Task.FromResult<ServicioResueltoDto?>(null);
    }

    public Task<DisponibilidadResueltaDto> ValidarDisponibilidadAsync(
        Guid guidServicio, DateTime fechaInicio, DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Middleware deshabilitado: ValidarDisponibilidad({Guid}) → disponible=true (modo dev).",
            guidServicio);
        return Task.FromResult(new DisponibilidadResueltaDto
        {
            Disponible = true,
            Motivo = "Middleware deshabilitado en este entorno."
        });
    }

    public Task PublicarEventoAsync(
        EventoAuditoriaDto evento, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Middleware deshabilitado: PublicarEvento {Tabla}/{Operacion} descartado (modo dev).",
            evento.TablaAfectada, evento.Operacion);
        return Task.CompletedTask;
    }
}
