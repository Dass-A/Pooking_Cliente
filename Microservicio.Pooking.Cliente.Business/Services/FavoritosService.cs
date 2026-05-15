using System.Text.Json;
using Microservicio.Pooking.Cliente.Business.Clients;
using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Favoritos;
using Microservicio.Pooking.Cliente.Business.Exceptions;
using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microservicio.Pooking.Cliente.Business.Mappers;
using Microservicio.Pooking.Cliente.Business.Validators;
using Microservicio.Pooking.Cliente.DataManagement.Interfaces;

namespace Microservicio.Pooking.Cliente.Business.Services;

public class FavoritosService : IFavoritosService
{
    private readonly IFavoritosDataService _favoritosDataService;
    private readonly IClienteDataService _clienteDataService;
    private readonly IMiddlewareGrpcClient _middlewareClient;

    public FavoritosService(
        IFavoritosDataService favoritosDataService,
        IClienteDataService clienteDataService,
        IMiddlewareGrpcClient middlewareClient)
    {
        _favoritosDataService = favoritosDataService;
        _clienteDataService = clienteDataService;
        _middlewareClient = middlewareClient;
    }

    public async Task<FavoritoResponse> ObtenerPorGuidAsync(Guid guidFavorito, CancellationToken cancellationToken = default)
    {
        var model = await _favoritosDataService.ObtenerPorGuidAsync(guidFavorito, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el favorito con GUID '{guidFavorito}'.");

        return FavoritosBusinessMapper.ToResponse(model);
    }

    public async Task<PagedResultado<FavoritoResponse>> ListarPorClienteAsync(
        Guid guidCliente, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        NormalizarPaginacion(ref pagina, ref tamanio);

        // Validar que el cliente exista
        _ = await _clienteDataService.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{guidCliente}'.");

        var paged = await _favoritosDataService.ListarPorClienteAsync(guidCliente, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(FavoritosBusinessMapper.ToResponse);

        return new PagedResultado<FavoritoResponse>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<FavoritoResponse> CrearAsync(CrearFavoritoRequest request, CancellationToken cancellationToken = default)
    {
        var errores = FavoritosValidator.ValidarCrear(request);
        if (errores.Count > 0)
            throw new ValidationException("Errores de validación al crear favorito.", errores);

        // Validar que el cliente exista
        _ = await _clienteDataService.ObtenerPorGuidAsync(request.GuidClienteRef, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{request.GuidClienteRef}'.");

        // Validar que no exista ya el mismo (cliente, servicio)
        if (await _favoritosDataService.ExisteFavoritoAsync(
            request.GuidClienteRef, request.GuidServicioRef, cancellationToken))
        {
            throw new BusinessException("Este servicio ya está marcado como favorito por el cliente.");
        }

        // ---------------------------------------------------------------------
        // RESUELTO (antes era TODO): Validación del servicio vía Middleware.
        // Si el Middleware está disponible y devuelve el servicio, validamos
        // que esté ACT. Si no está disponible (modo dev), se confía en el frontend.
        // ---------------------------------------------------------------------
        var servicioResuelto = await _middlewareClient.ResolverServicioAsync(
            request.GuidServicioRef, cancellationToken);

        if (servicioResuelto is not null &&
            !string.Equals(servicioResuelto.Estado, "ACT", StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException(
                $"No se puede marcar como favorito un servicio inactivo (estado: {servicioResuelto.Estado}).");
        }

        var dataModel = FavoritosBusinessMapper.ToDataModelFromCreate(request);
        var creado = await _favoritosDataService.CrearAsync(dataModel, cancellationToken);

        // Publicar evento INSERT
        await _middlewareClient.PublicarEventoAsync(new EventoAuditoriaDto
        {
            TablaAfectada = "favoritos",
            Operacion = TipoOperacionAuditoria.Insert,
            IdRegistro = creado.GuidFavorito.ToString(),
            DatosNuevos = JsonSerializer.Serialize(new
            {
                creado.GuidFavorito,
                creado.GuidClienteRef,
                creado.GuidServicioRef,
                creado.Alias,
                creado.Estado
            }),
            ServicioOrigen = "Cliente"
        }, cancellationToken);

        return FavoritosBusinessMapper.ToResponse(creado);
    }

    public async Task EliminarAsync(Guid guidFavorito, CancellationToken cancellationToken = default)
    {
        var existente = await _favoritosDataService.ObtenerPorGuidAsync(guidFavorito, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el favorito con GUID '{guidFavorito}'.");

        await _favoritosDataService.EliminarLogicoAsync(guidFavorito, cancellationToken);

        // Publicar evento DELETE
        await _middlewareClient.PublicarEventoAsync(new EventoAuditoriaDto
        {
            TablaAfectada = "favoritos",
            Operacion = TipoOperacionAuditoria.Delete,
            IdRegistro = existente.GuidFavorito.ToString(),
            DatosAnteriores = JsonSerializer.Serialize(new
            {
                existente.GuidFavorito,
                existente.GuidClienteRef,
                existente.GuidServicioRef,
                existente.Alias,
                existente.Estado
            }),
            ServicioOrigen = "Cliente"
        }, cancellationToken);
    }

    private static void NormalizarPaginacion(ref int pagina, ref int tamanio)
    {
        if (pagina < 1) pagina = 1;
        if (tamanio < 1) tamanio = 10;
        if (tamanio > 100) tamanio = 100;
    }
}
