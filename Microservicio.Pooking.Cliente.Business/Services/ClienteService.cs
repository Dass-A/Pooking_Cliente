using System.Text.Json;
using Microservicio.Pooking.Cliente.Business.Clients;
using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Cliente;
using Microservicio.Pooking.Cliente.Business.Exceptions;
using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microservicio.Pooking.Cliente.Business.Mappers;
using Microservicio.Pooking.Cliente.Business.Validators;
using Microservicio.Pooking.Cliente.DataManagement.Interfaces;

namespace Microservicio.Pooking.Cliente.Business.Services;

/// <summary>
/// Servicio de negocio para clientes.
/// Aplica validaciones y reglas de negocio antes de invocar a DataManagement.
/// Nunca accede directamente a EF Core ni a repositorios.
/// Publica eventos al Middleware tras cada operación de escritura exitosa.
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IClienteDataService _clienteDataService;
    private readonly IMiddlewareGrpcClient _middlewareClient;

    public ClienteService(
        IClienteDataService clienteDataService,
        IMiddlewareGrpcClient middlewareClient)
    {
        _clienteDataService = clienteDataService;
        _middlewareClient = middlewareClient;
    }

    // -------------------------------------------------------------------------
    // Consultas
    // -------------------------------------------------------------------------

    public async Task<ClienteResponse> ObtenerPorGuidAsync(Guid guidCliente, CancellationToken cancellationToken = default)
    {
        var model = await _clienteDataService.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{guidCliente}'.");

        return ClienteBusinessMapper.ToResponse(model);
    }

    public async Task<ClienteResponse> ObtenerPorUsuarioGuidRefAsync(Guid usuarioGuidRef, CancellationToken cancellationToken = default)
    {
        var model = await _clienteDataService.ObtenerPorUsuarioGuidRefAsync(usuarioGuidRef, cancellationToken)
            ?? throw new NotFoundException($"No se encontró un cliente vinculado al usuario '{usuarioGuidRef}'.");

        return ClienteBusinessMapper.ToResponse(model);
    }

    public async Task<ClienteResponse> ObtenerPorNumeroIdentificacionAsync(
        string tipoIdentificacion, string numeroIdentificacion, CancellationToken cancellationToken = default)
    {
        var model = await _clienteDataService.ObtenerPorNumeroIdentificacionAsync(
            tipoIdentificacion, numeroIdentificacion, cancellationToken)
            ?? throw new NotFoundException($"No se encontró cliente con identificación '{numeroIdentificacion}'.");

        return ClienteBusinessMapper.ToResponse(model);
    }

    public async Task<PagedResultado<ClienteResponse>> ListarPaginadoAsync(
        int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        NormalizarPaginacion(ref pagina, ref tamanio);

        var paged = await _clienteDataService.ListarPaginadoAsync(pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ClienteBusinessMapper.ToResponse);

        return new PagedResultado<ClienteResponse>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    public async Task<PagedResultado<ClienteResponse>> ListarPorEstadoAsync(
        string estado, int pagina, int tamanio, CancellationToken cancellationToken = default)
    {
        NormalizarPaginacion(ref pagina, ref tamanio);

        var paged = await _clienteDataService.ListarPorEstadoAsync(estado, pagina, tamanio, cancellationToken);
        var items = paged.Items.Select(ClienteBusinessMapper.ToResponse);

        return new PagedResultado<ClienteResponse>(items, paged.TotalRegistros, paged.PaginaActual, paged.TamanoPagina);
    }

    // -------------------------------------------------------------------------
    // Comandos
    // -------------------------------------------------------------------------

    public async Task<ClienteResponse> CrearAsync(CrearClienteRequest request, CancellationToken cancellationToken = default)
    {
        var errores = ClienteValidator.ValidarCrear(request);
        if (errores.Count > 0)
            throw new ValidationException("Errores de validación al crear cliente.", errores);

        // Reglas de unicidad
        if (await _clienteDataService.ExisteCorreoAsync(request.Correo, cancellationToken))
            throw new BusinessException($"Ya existe un cliente con el correo '{request.Correo}'.");

        if (await _clienteDataService.ExisteNumeroIdentificacionAsync(
            request.TipoIdentificacion, request.NumeroIdentificacion, cancellationToken))
            throw new BusinessException(
                $"Ya existe un cliente con identificación {request.TipoIdentificacion} '{request.NumeroIdentificacion}'.");

        if (await _clienteDataService.ExisteUsuarioVinculadoAsync(request.UsuarioGuidRef, cancellationToken))
            throw new BusinessException($"Ya existe un cliente vinculado al usuario '{request.UsuarioGuidRef}'.");

        var dataModel = ClienteBusinessMapper.ToDataModelFromCreate(request);
        var creado = await _clienteDataService.CrearAsync(dataModel, cancellationToken);

        await _middlewareClient.PublicarEventoAsync(new EventoAuditoriaDto
        {
            TablaAfectada = "cliente",
            Operacion = TipoOperacionAuditoria.Insert,
            IdRegistro = creado.GuidCliente.ToString(),
            DatosNuevos = JsonSerializer.Serialize(new
            {
                creado.GuidCliente,
                creado.UsuarioGuidRef,
                creado.Correo,
                creado.TipoIdentificacion,
                creado.NumeroIdentificacion,
                creado.Estado
            }),
            ServicioOrigen = "Cliente"
        }, cancellationToken);

        return ClienteBusinessMapper.ToResponse(creado);
    }

    public async Task<ClienteResponse> ActualizarAsync(
        Guid guidCliente, ActualizarClienteRequest request, CancellationToken cancellationToken = default)
    {
        var errores = ClienteValidator.ValidarActualizar(request);
        if (errores.Count > 0)
            throw new ValidationException("Errores de validación al actualizar cliente.", errores);

        var existente = await _clienteDataService.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{guidCliente}'.");

        // Verificar unicidad si cambió el correo o la identificación
        if (!string.Equals(existente.Correo, request.Correo, StringComparison.OrdinalIgnoreCase) &&
            await _clienteDataService.ExisteCorreoAsync(request.Correo, cancellationToken))
        {
            throw new BusinessException($"Ya existe otro cliente con el correo '{request.Correo}'.");
        }

        if ((!string.Equals(existente.TipoIdentificacion, request.TipoIdentificacion, StringComparison.OrdinalIgnoreCase) ||
             !string.Equals(existente.NumeroIdentificacion, request.NumeroIdentificacion, StringComparison.OrdinalIgnoreCase)) &&
            await _clienteDataService.ExisteNumeroIdentificacionAsync(
                request.TipoIdentificacion, request.NumeroIdentificacion, cancellationToken))
        {
            throw new BusinessException(
                $"Ya existe otro cliente con identificación {request.TipoIdentificacion} '{request.NumeroIdentificacion}'.");
        }

        var dataModel = ClienteBusinessMapper.ToDataModelFromUpdate(guidCliente, request, existente);
        var actualizado = await _clienteDataService.ActualizarAsync(dataModel, cancellationToken);

        await _middlewareClient.PublicarEventoAsync(new EventoAuditoriaDto
        {
            TablaAfectada = "cliente",
            Operacion = TipoOperacionAuditoria.Update,
            IdRegistro = actualizado.GuidCliente.ToString(),
            DatosAnteriores = JsonSerializer.Serialize(new
            {
                existente.Correo, existente.Telefono, existente.Direccion,
                existente.Nombres, existente.Apellidos
            }),
            DatosNuevos = JsonSerializer.Serialize(new
            {
                actualizado.Correo, actualizado.Telefono, actualizado.Direccion,
                actualizado.Nombres, actualizado.Apellidos
            }),
            ServicioOrigen = "Cliente"
        }, cancellationToken);

        return ClienteBusinessMapper.ToResponse(actualizado);
    }

    public async Task EliminarLogicoAsync(Guid guidCliente, CancellationToken cancellationToken = default)
    {
        var existente = await _clienteDataService.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{guidCliente}'.");

        await _clienteDataService.EliminarLogicoAsync(guidCliente, cancellationToken);

        await _middlewareClient.PublicarEventoAsync(new EventoAuditoriaDto
        {
            TablaAfectada = "cliente",
            Operacion = TipoOperacionAuditoria.Delete,
            IdRegistro = existente.GuidCliente.ToString(),
            DatosAnteriores = JsonSerializer.Serialize(new
            {
                existente.GuidCliente,
                existente.UsuarioGuidRef,
                existente.Correo,
                existente.TipoIdentificacion,
                existente.NumeroIdentificacion,
                existente.Estado
            }),
            ServicioOrigen = "Cliente"
        }, cancellationToken);
    }

    public async Task CambiarEstadoAsync(Guid guidCliente, string nuevoEstado, CancellationToken cancellationToken = default)
    {
        var estadosValidos = new[] { "ACT", "INA", "SUS" };
        if (!estadosValidos.Contains(nuevoEstado, StringComparer.OrdinalIgnoreCase))
            throw new ValidationException(
                "Estado inválido.",
                new[] { "El estado debe ser ACT, INA o SUS." });

        var existente = await _clienteDataService.ObtenerPorGuidAsync(guidCliente, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el cliente con GUID '{guidCliente}'.");

        var estadoAnterior = existente.Estado;
        var estadoNuevo = nuevoEstado.ToUpperInvariant();

        await _clienteDataService.CambiarEstadoAsync(guidCliente, estadoNuevo, cancellationToken);

        await _middlewareClient.PublicarEventoAsync(new EventoAuditoriaDto
        {
            TablaAfectada = "cliente",
            Operacion = TipoOperacionAuditoria.Update,
            IdRegistro = existente.GuidCliente.ToString(),
            DatosAnteriores = JsonSerializer.Serialize(new { Estado = estadoAnterior }),
            DatosNuevos = JsonSerializer.Serialize(new { Estado = estadoNuevo }),
            ServicioOrigen = "Cliente"
        }, cancellationToken);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static void NormalizarPaginacion(ref int pagina, ref int tamanio)
    {
        if (pagina < 1) pagina = 1;
        if (tamanio < 1) tamanio = 10;
        if (tamanio > 100) tamanio = 100;
    }
}
