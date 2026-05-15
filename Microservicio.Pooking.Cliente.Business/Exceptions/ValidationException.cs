namespace Microservicio.Pooking.Cliente.Business.Exceptions;

/// <summary>
/// Excepción lanzada cuando uno o más campos no cumplen las reglas de negocio.
/// La API la traduce a HTTP 400 Bad Request.
/// </summary>
public class ValidationException : BusinessException
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(string message, IReadOnlyCollection<string>? errors = null)
        : base(message)
    {
        Errors = errors ?? Array.Empty<string>();
    }
}
