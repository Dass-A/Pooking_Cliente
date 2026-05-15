using Asp.Versioning;
using Microservicio.Pooking.Cliente.Api.Models.Common;
using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Reservas;
using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Pooking.Cliente.Api.Controllers.V1;

/// <summary>
/// Reservas: solo creación y consulta.
/// Las transiciones de estado (CANC, COMP) las realiza otro microservicio (Pago/Proveedor).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reservas")]
public class ReservasController : ControllerBase
{
    private readonly IReservasService _reservasService;

    public ReservasController(IReservasService reservasService)
    {
        _reservasService = reservasService;
    }

    /// <summary>Obtiene una reserva por su GUID.</summary>
    [HttpGet("{guidReserva:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ReservaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorGuid(Guid guidReserva, CancellationToken cancellationToken)
    {
        var result = await _reservasService.ObtenerPorGuidAsync(guidReserva, cancellationToken);
        return Ok(ApiResponse<ReservaResponse>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Lista las reservas de un cliente, paginadas.</summary>
    [HttpGet("cliente/{guidCliente:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResultado<ReservaResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorCliente(
        Guid guidCliente,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanio = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _reservasService.ListarPorClienteAsync(guidCliente, pagina, tamanio, cancellationToken);
        return Ok(ApiResponse<PagedResultado<ReservaResponse>>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Lista reservas por estado (PEND/CONF/CANC/COMP), paginadas.</summary>
    [HttpGet("estado/{estado}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResultado<ReservaResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorEstado(
        string estado,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanio = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _reservasService.ListarPorEstadoAsync(estado, pagina, tamanio, cancellationToken);
        return Ok(ApiResponse<PagedResultado<ReservaResponse>>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Crea una nueva reserva (estado inicial CONF, con snapshot inmutable del servicio).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReservaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearReservaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _reservasService.CrearAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(ObtenerPorGuid),
            new { guidReserva = result.GuidReserva, version = "1.0" },
            ApiResponse<ReservaResponse>.Ok(result, "Reserva creada exitosamente."));
    }
}
