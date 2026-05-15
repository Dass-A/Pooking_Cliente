using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Cliente.DataAccess.Context;

/// <summary>
/// DbContext del microservicio Cliente.
/// Expone únicamente las 3 tablas del dominio: cliente, reservas, favoritos.
/// NO contiene DbSet de Auth, Catálogo ni Auditoría — esos viven en sus propios microservicios.
/// </summary>
public class ClienteDbContext : DbContext
{
    public ClienteDbContext(DbContextOptions<ClienteDbContext> options)
        : base(options) { }

    public DbSet<ClienteEntity> Clientes => Set<ClienteEntity>();
    public DbSet<ReservasEntity> Reservas => Set<ReservasEntity>();
    public DbSet<FavoritosEntity> Favoritos => Set<FavoritosEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClienteDbContext).Assembly);
    }
}
