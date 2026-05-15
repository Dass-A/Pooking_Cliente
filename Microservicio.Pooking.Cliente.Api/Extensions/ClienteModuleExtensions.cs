using Microservicio.Pooking.Cliente.Business.Interfaces;
using Microservicio.Pooking.Cliente.Business.Services;
using Microservicio.Pooking.Cliente.DataAccess.Context;
using Microservicio.Pooking.Cliente.DataManagement.Interfaces;
using Microservicio.Pooking.Cliente.DataManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Cliente.Api.Extensions;

/// <summary>
/// Registra todas las capas del microservicio Cliente:
/// Capa 1 — DbContext (DataAccess)
/// Capa 2 — UnitOfWork + DataServices (DataManagement)
/// Capa 3 — Services de negocio (Business)
/// </summary>
public static class ClienteModuleExtensions
{
    public static IServiceCollection AddClienteModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Capa 1 — DbContext (PostgreSQL)
        services.AddDbContext<ClienteDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Capa 2 — Unit of Work y servicios de datos
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClienteDataService, ClienteDataService>();
        services.AddScoped<IReservasDataService, ReservasDataService>();
        services.AddScoped<IFavoritosDataService, FavoritosDataService>();

        // Capa 3 — Servicios de negocio
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IReservasService, ReservasService>();
        services.AddScoped<IFavoritosService, FavoritosService>();

        return services;
    }
}
