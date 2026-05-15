using Microservicio.Pooking.Cliente.Business.DTOs.Cliente;

namespace Microservicio.Pooking.Cliente.Business.Validators;

/// <summary>
/// Reglas de validación para creación/actualización de cliente.
/// Implementación manual (sin FluentValidation) — alineada con el patrón del monolito.
/// </summary>
public static class ClienteValidator
{
    private static readonly HashSet<string> TiposIdentificacionValidos = new(StringComparer.OrdinalIgnoreCase)
    { "CI", "RUC", "PASS", "EXT" };

    private static readonly HashSet<string> EstadosValidos = new(StringComparer.OrdinalIgnoreCase)
    { "ACT", "INA", "SUS" };

    public static List<string> ValidarCrear(CrearClienteRequest request)
    {
        var errores = new List<string>();

        if (request.UsuarioGuidRef == Guid.Empty)
            errores.Add("El UsuarioGuidRef es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.TipoIdentificacion))
            errores.Add("El tipo de identificación es obligatorio.");
        else if (!TiposIdentificacionValidos.Contains(request.TipoIdentificacion))
            errores.Add("El tipo de identificación debe ser CI, RUC, PASS o EXT.");

        if (string.IsNullOrWhiteSpace(request.NumeroIdentificacion))
            errores.Add("El número de identificación es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Correo))
            errores.Add("El correo es obligatorio.");
        else if (!request.Correo.Contains('@'))
            errores.Add("El correo no tiene un formato válido.");

        // Persona natural vs jurídica
        var esJuridica = string.Equals(request.TipoIdentificacion, "RUC", StringComparison.OrdinalIgnoreCase);
        if (esJuridica)
        {
            if (string.IsNullOrWhiteSpace(request.RazonSocial))
                errores.Add("Para personas jurídicas (RUC) la razón social es obligatoria.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.Nombres) && string.IsNullOrWhiteSpace(request.Apellidos))
                errores.Add("Para personas naturales, al menos nombres o apellidos deben estar presentes.");
        }

        return errores;
    }

    public static List<string> ValidarActualizar(ActualizarClienteRequest request)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(request.TipoIdentificacion))
            errores.Add("El tipo de identificación es obligatorio.");
        else if (!TiposIdentificacionValidos.Contains(request.TipoIdentificacion))
            errores.Add("El tipo de identificación debe ser CI, RUC, PASS o EXT.");

        if (string.IsNullOrWhiteSpace(request.NumeroIdentificacion))
            errores.Add("El número de identificación es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Correo))
            errores.Add("El correo es obligatorio.");
        else if (!request.Correo.Contains('@'))
            errores.Add("El correo no tiene un formato válido.");

        if (!EstadosValidos.Contains(request.Estado))
            errores.Add("El estado debe ser ACT, INA o SUS.");

        return errores;
    }
}
