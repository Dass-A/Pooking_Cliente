namespace Microservicio.Pooking.Cliente.Business.Exceptions;

/// <summary>
/// Recurso solicitado no existe o no está disponible para la operación.
/// La API la traduce a HTTP 404 Not Found.
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string mensaje, string? codigo = null) : base(mensaje, codigo) { }
}
