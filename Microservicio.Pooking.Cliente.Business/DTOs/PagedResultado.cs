namespace Microservicio.Pooking.Cliente.Business.DTOs;

/// <summary>
/// Resultado paginado expuesto en la capa Business hacia la API.
/// </summary>
public class PagedResultado<T>
{
    public IReadOnlyList<T> Items { get; init; }
    public int PaginaActual { get; init; }
    public int TamanoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }

    public PagedResultado(IEnumerable<T> items, int totalRegistros, int paginaActual, int tamanoPagina)
    {
        Items = items.ToList().AsReadOnly();
        TotalRegistros = totalRegistros;
        PaginaActual = paginaActual;
        TamanoPagina = tamanoPagina;
        TotalPaginas = tamanoPagina > 0
            ? (int)Math.Ceiling((double)totalRegistros / tamanoPagina)
            : 0;
    }
}
