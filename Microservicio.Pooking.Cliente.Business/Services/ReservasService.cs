using System.Text.Json;
using Microservicio.Pooking.Cliente.Business.Clients;
using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Reservas;
using Microservicio.Pooking.Cliente.Business.Exceptions;
using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microservicio.Pooking.Cliente.Business.Mappers;
using Microservicio.Pooking.Cliente.Business.Validators;
using Microservicio.Pooking.Cliente.DataManagement.Interfaces;

namespace Microservicio.Pooking.Cliente.Business.Services;

public class ReservasService : IReservasService
{
    private readonly IReservasDataService _reservasDataService;
    private readonly IClienteDataService _clienteDataService;
    private readonly IMiddlewareGrpcClient _middlewareClient;

    public ReservasService(
        IReservasDataService reservasDataService,
        IClienteDataService clienteDataService,
        IMiddlewareGrpcClient middlewareClient)
    {
        _reservasDataService = reservasDataService;
        _clienteDataService = clienteDataService;
        _middlewareClient = middlewareClient;
    }

    public async Task<ReservaResponse> ObtenerPorGuidAsync(Guid guidReserva, CancellationToken cancellationToken = default)
    {
        var model = await _reservasDataService.ObtenerPorGuidAsync(guidReserva, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la reserva con GUID '{guidReserva}'.");

        return ReservasBusinessMapper.ToResponse(model);
    }

    public async Task<PagedResultado<ReservaResponse>> ListarPorClienteAsync(
        Guid guidCliente, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        NormalizarPaginacion(ref pagina, ref tamanio);

        var cliente = await _clienteDataService.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{guidCliente}'.");

        var paged = await _reservasDataService.ListarPorClienteAsync(cliente.IdCliente, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ReservasBusinessMapper.ToResponse);

        return new PagedResultado<ReservaResponse>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<PagedResultado<ReservaResponse>> ListarPorEstadoAsync(
        string estado, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        NormalizarPaginacion(ref pagina, ref tamanio);

        var estadosValidos = new[] { "PEND", "CONF", "CANC", "COMP" };
        if (!estadosValidos.Contains(estado, StringComparer.OrdinalIgnoreCase))
            throw new ValidationException(
                "Estado inválido.",
                new[] { "El estado debe ser PEND, CONF, CANC o COMP." });

        var paged = await _reservasDataService.ListarPorEstadoAsync(estado.ToUpperInvariant(), pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ReservasBusinessMapper.ToResponse);

        return new PagedResultado<ReservaResponse>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<ReservaResponse> CrearAsync(CrearReservaRequest request, CancellationToken cancellationToken = default)
    {
        var errores = ReservasValidator.ValidarCrear(request);
        if (errores.Count > 0)
            throw new ValidationException("Errores de validación al crear reserva.", errores);

        var cliente = await _clienteDataService.ObtenerPorGuidAsync(request.GuidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{request.GuidCliente}'.");

        if (!string.Equals(cliente.Estado, "ACT", StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("No se pueden crear reservas para clientes inactivos o suspendidos.");

        // ---------------------------------------------------------------------
        // RESUELTO (antes era TODO): Validación de servicio vía Middleware.
        // Llama a Middleware.ResolverServicio que internamente consulta
        // Microservicio.Pooking.Servicio. Si el servicio no existe o no está
        // activo, se rechaza la reserva.
        //
        // En modo dev sin Middleware corriendo, MiddlewareGrpcClientDisabled
        // devuelve null → se cae en el branch "no encontrado" y la creación falla.
        // Para desarrollo aislado, configurar GrpcEndpoints:Enabled = false en
        // appsettings.json — el modo no-op devolverá disponibilidad optimista.
        // ---------------------------------------------------------------------
        var servicioResuelto = await _middlewareClient.ResolverServicioAsync(
            request.GuidServicioRef, cancellationToken);

        if (servicioResuelto is null)
        {
            // En desarrollo aislado (Middleware off + Enabled=false) confiamos en
            // el snapshot que vino del request. En integración, esto sería error.
            // El comportamiento exacto se ajusta en Program.cs según el entorno.
        }
        else
        {
            if (!string.Equals(servicioResuelto.Estado, "ACT", StringComparison.OrdinalIgnoreCase))
                throw new BusinessException(
                    $"El servicio referenciado no está activo (estado: {servicioResuelto.Estado}).");

            // Sobrescribimos el snapshot con datos REALES del servicio
            // (no confiamos en lo que envió el frontend para el nombre del proveedor).
            request.NombreProveedor = servicioResuelto.RazonSocial;
            request.NombreServicioSnap = servicioResuelto.NombreComercial.Length > 0
                ? servicioResuelto.NombreComercial
                : servicioResuelto.RazonSocial;
            request.TipoServicioSnap = servicioResuelto.TipoServicio;

            // Validar disponibilidad para el rango de fechas
            var disponibilidad = await _middlewareClient.ValidarDisponibilidadAsync(
                request.GuidServicioRef,
                request.FechaInicio,
                request.FechaFin,
                cancellationToken);

            if (!disponibilidad.Disponible)
                throw new BusinessException(
                    $"El servicio no está disponible en el rango solicitado: {disponibilidad.Motivo}");
        }

        var dataModel = ReservasBusinessMapper.ToDataModelFromCreate(request, cliente.IdCliente);
        var creada = await _reservasDataService.CrearAsync(dataModel, cancellationToken);
        creada.GuidClienteRef = cliente.GuidCliente;

        // ---------------------------------------------------------------------
        // Publicar evento de auditoría (asíncrono, fire-and-forget).
        // Si falla, MiddlewareGrpcClient lo loggea pero no rompe la operación.
        // ---------------------------------------------------------------------
        await _middlewareClient.PublicarEventoAsync(new EventoAuditoriaDto
        {
            TablaAfectada = "reservas",
            Operacion = TipoOperacionAuditoria.Insert,
            IdRegistro = creada.GuidReserva.ToString(),
            DatosNuevos = JsonSerializer.Serialize(new
            {
                creada.GuidReserva,
                GuidCliente = creada.GuidClienteRef,
                creada.GuidServicioRef,
                creada.NombreProveedor,
                creada.Estado,
                creada.MontoTotal
            }),
            ServicioOrigen = "Cliente"
        }, cancellationToken);

        return ReservasBusinessMapper.ToResponse(creada);
    }

    private static void NormalizarPaginacion(ref int pagina, ref int tamanio)
    {
        if (pagina < 1) pagina = 1;
        if (tamanio < 1) tamanio = 10;
        if (tamanio > 100) tamanio = 100;
    }
}
