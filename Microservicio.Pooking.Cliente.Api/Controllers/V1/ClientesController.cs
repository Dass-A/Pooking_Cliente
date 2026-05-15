using Asp.Versioning;
using Microservicio.Pooking.Cliente.Api.Models.Common;
using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Cliente;
using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Pooking.Cliente.Api.Controllers.V1;

/// <summary>
/// CRUD de clientes del microservicio Cliente.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    /// <summary>Obtiene un cliente por su GUID público.</summary>
    [HttpGet("{guidCliente:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorGuid(Guid guidCliente, CancellationToken cancellationToken)
    {
        var result = await _clienteService.ObtenerPorGuidAsync(guidCliente, cancellationToken);
        return Ok(ApiResponse<ClienteResponse>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Obtiene el cliente vinculado a un UsuarioGuidRef del dominio Auth.</summary>
    [HttpGet("usuario-guid/{usuarioGuidRef:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorUsuarioGuidRef(Guid usuarioGuidRef, CancellationToken cancellationToken)
    {
        var result = await _clienteService.ObtenerPorUsuarioGuidRefAsync(usuarioGuidRef, cancellationToken);
        return Ok(ApiResponse<ClienteResponse>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Obtiene un cliente por tipo y número de identificación.</summary>
    [HttpGet("identificacion/{tipoIdentificacion}/{numeroIdentificacion}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorNumeroIdentificacion(
        string tipoIdentificacion,
        string numeroIdentificacion,
        CancellationToken cancellationToken)
    {
        var result = await _clienteService.ObtenerPorNumeroIdentificacionAsync(
            tipoIdentificacion, numeroIdentificacion, cancellationToken);
        return Ok(ApiResponse<ClienteResponse>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Lista clientes paginados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResultado<ClienteResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanio = 10,
        [FromQuery] string? estado = null,
        CancellationToken cancellationToken = default)
    {
        var result = string.IsNullOrEmpty(estado)
            ? await _clienteService.ListarPaginadoAsync(pagina, tamanio, cancellationToken)
            : await _clienteService.ListarPorEstadoAsync(estado, pagina, tamanio, cancellationToken);

        return Ok(ApiResponse<PagedResultado<ClienteResponse>>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Crea un nuevo cliente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ClienteResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearClienteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _clienteService.CrearAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(ObtenerPorGuid),
            new { guidCliente = result.GuidCliente, version = "1.0" },
            ApiResponse<ClienteResponse>.Ok(result, "Cliente creado exitosamente."));
    }

    /// <summary>Actualiza un cliente existente.</summary>
    [HttpPut("{guidCliente:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(
        Guid guidCliente,
        [FromBody] ActualizarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _clienteService.ActualizarAsync(guidCliente, request, cancellationToken);
        return Ok(ApiResponse<ClienteResponse>.Ok(result, "Cliente actualizado exitosamente."));
    }

    /// <summary>Eliminación lógica de un cliente.</summary>
    [HttpDelete("{guidCliente:guid}")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid guidCliente, CancellationToken cancellationToken)
    {
        await _clienteService.EliminarLogicoAsync(guidCliente, cancellationToken);
        return NoContent();
    }

    /// <summary>Cambia el estado de un cliente (ACT/INA/SUS).</summary>
    [HttpPatch("{guidCliente:guid}/estado/{nuevoEstado}")]
    [Authorize(Roles = "ADMINISTRADOR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarEstado(
        Guid guidCliente,
        string nuevoEstado,
        CancellationToken cancellationToken)
    {
        await _clienteService.CambiarEstadoAsync(guidCliente, nuevoEstado, cancellationToken);
        return NoContent();
    }
}
