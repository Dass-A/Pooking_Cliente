namespace Microservicio.Pooking.Cliente.DataManagement.Models;

/// <summary>
/// Resultado paginado expuesto desde DataManagement al Business.
/// Espeja PagedResult del DataAccess pero sin acoplar Business a EF Core.
/// </summary>
public class PagedDataResult<T>
{
    public IReadOnlyList<T> Items { get; init; }
    public int PaginaActual { get; init; }
    public int TamanoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }

    public PagedDataResult(IEnumerable<T> items, int totalRegistros, int paginaActual, int tamanoPagina)
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
