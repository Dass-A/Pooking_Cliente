namespace Microservicio.Pooking.Cliente.DataAccess.Common;

/// <summary>
/// Envuelve una página de resultados junto con los metadatos de paginación.
/// Tipo de retorno estándar de cualquier consulta paginada en la DAL.
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; }
    public int PaginaActual { get; init; }
    public int TamanoPagina { get; init; }
    public int TotalRegistros { get; init; }

    public int TotalPaginas => TamanoPagina > 0
        ? (int)Math.Ceiling((double)TotalRegistros / TamanoPagina)
        : 0;

    public bool TienePaginaAnterior => PaginaActual > 1;
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

    public PagedResult(
        IEnumerable<T> items,
        int totalRegistros,
        int paginaActual,
        int tamanoPagina)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(paginaActual, 1, nameof(paginaActual));
        ArgumentOutOfRangeException.ThrowIfLessThan(tamanoPagina, 1, nameof(tamanoPagina));
        ArgumentOutOfRangeException.ThrowIfNegative(totalRegistros, nameof(totalRegistros));

        Items = items.ToList().AsReadOnly();
        TotalRegistros = totalRegistros;
        PaginaActual = paginaActual;
        TamanoPagina = tamanoPagina;
    }

    public static PagedResult<T> Vacio(int paginaActual, int tamanoPagina) =>
        new([], totalRegistros: 0, paginaActual, tamanoPagina);
}
