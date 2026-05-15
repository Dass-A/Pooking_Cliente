using Microservicio.Pooking.Cliente.Business.DTOs.Reservas;

namespace Microservicio.Pooking.Cliente.Business.Validators;

public static class ReservasValidator
{
    public static List<string> ValidarCrear(CrearReservaRequest request)
    {
        var errores = new List<string>();

        if (request.GuidCliente == Guid.Empty)
            errores.Add("El GuidCliente es obligatorio.");

        if (request.GuidServicioRef == Guid.Empty)
            errores.Add("El GuidServicioRef es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.NombreServicioSnap))
            errores.Add("El nombre del servicio (snapshot) es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.TipoServicioSnap))
            errores.Add("El tipo del servicio (snapshot) es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.NombreProveedor))
            errores.Add("El nombre del proveedor es obligatorio.");
        else if (request.NombreProveedor.Length > 200)
            errores.Add("El nombre del proveedor no puede exceder 200 caracteres.");

        if (string.IsNullOrWhiteSpace(request.IdReservaExterna))
            errores.Add("El id de reserva externa es obligatorio.");

        if (request.FechaInicio == default)
            errores.Add("La fecha de inicio es obligatoria.");

        if (request.FechaFin.HasValue && request.FechaFin < request.FechaInicio)
            errores.Add("La fecha fin no puede ser anterior a la fecha de inicio.");

        if (request.MontoTotal < 0)
            errores.Add("El monto total no puede ser negativo.");

        if (string.IsNullOrWhiteSpace(request.Moneda) || request.Moneda.Length != 3)
            errores.Add("La moneda debe ser un código ISO de 3 caracteres (ej: USD).");

        return errores;
    }
}
