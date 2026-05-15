using Asp.Versioning;
using Microservicio.Pooking.Cliente.Api.Models.Common;
using Microservicio.Pooking.Cliente.Business.DTOs;
using Microservicio.Pooking.Cliente.Business.DTOs.Favoritos;
using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Pooking.Cliente.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/favoritos")]
public class FavoritosController : ControllerBase
{
    private readonly IFavoritosService _favoritosService;

    public FavoritosController(IFavoritosService favoritosService)
    {
        _favoritosService = favoritosService;
    }

    /// <summary>Obtiene un favorito por su GUID.</summary>
    [HttpGet("{guidFavorito:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FavoritoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorGuid(Guid guidFavorito, CancellationToken cancellationToken)
    {
        var result = await _favoritosService.ObtenerPorGuidAsync(guidFavorito, cancellationToken);
        return Ok(ApiResponse<FavoritoResponse>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Lista los favoritos de un cliente, paginados.</summary>
    [HttpGet("cliente/{guidCliente:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResultado<FavoritoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorCliente(
        Guid guidCliente,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanio = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _favoritosService.ListarPorClienteAsync(guidCliente, pagina, tamanio, cancellationToken);
        return Ok(ApiResponse<PagedResultado<FavoritoResponse>>.Ok(result, "Consulta exitosa."));
    }

    /// <summary>Marca un servicio como favorito de un cliente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FavoritoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear(
        [FromBody] CrearFavoritoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _favoritosService.CrearAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(ObtenerPorGuid),
            new { guidFavorito = result.GuidFavorito, version = "1.0" },
            ApiResponse<FavoritoResponse>.Ok(result, "Favorito creado exitosamente."));
    }

    /// <summary>Elimina (lógicamente) un favorito.</summary>
    [HttpDelete("{guidFavorito:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid guidFavorito, CancellationToken cancellationToken)
    {
        await _favoritosService.EliminarAsync(guidFavorito, cancellationToken);
        return NoContent();
    }
}
