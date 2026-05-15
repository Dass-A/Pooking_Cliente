using Microservicio.Pooking.Cliente.Business.DTOs.Favoritos;

namespace Microservicio.Pooking.Cliente.Business.Validators;

public static class FavoritosValidator
{
    public static List<string> ValidarCrear(CrearFavoritoRequest request)
    {
        var errores = new List<string>();

        if (request.GuidClienteRef == Guid.Empty)
            errores.Add("El GuidClienteRef es obligatorio.");

        if (request.GuidServicioRef == Guid.Empty)
            errores.Add("El GuidServicioRef es obligatorio.");

        if (!string.IsNullOrEmpty(request.Alias) && request.Alias.Length > 100)
            errores.Add("El alias no puede exceder 100 caracteres.");

        return errores;
    }
}
