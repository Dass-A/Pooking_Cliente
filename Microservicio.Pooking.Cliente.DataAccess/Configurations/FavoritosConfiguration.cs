using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Pooking.Cliente.DataAccess.Configurations;

/// <summary>
/// Mapping EF Core para FavoritosEntity → tabla booking.favoritos.
/// Ambas referencias (cliente, servicio) son lógicas cross-dominio.
/// </summary>
public class FavoritosConfiguration : IEntityTypeConfiguration<FavoritosEntity>
{
    public void Configure(EntityTypeBuilder<FavoritosEntity> builder)
    {
        builder.ToTable("favoritos", "booking");

        builder.HasKey(e => e.IdFavorito);

        builder.Property(e => e.IdFavorito)
            .HasColumnName("id_favorito")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.GuidFavorito)
            .HasColumnName("guid_favorito")
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        // [2] Referencias lógicas
        builder.Property(e => e.GuidClienteRef)
            .HasColumnName("guid_cliente_ref")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(e => e.GuidServicioRef)
            .HasColumnName("guid_servicio_ref")
            .HasColumnType("uuid")
            .IsRequired();

        // [3] Datos funcionales
        builder.Property(e => e.Alias)
            .HasColumnName("alias")
            .HasMaxLength(100);

        // [4] Estado y ciclo de vida
        builder.Property(e => e.Estado)
            .HasColumnName("estado")
            .HasColumnType("char(3)")
            .HasDefaultValue("ACT")
            .IsRequired();

        builder.Property(e => e.EsEliminado)
            .HasColumnName("es_eliminado")
            .HasDefaultValue(false)
            .IsRequired();

        // [5] Auditoría
        builder.Property(e => e.CreadoPorUsuario)
            .HasColumnName("creado_por_usuario")
            .HasMaxLength(150);

        builder.Property(e => e.FechaRegistroUtc)
            .HasColumnName("fecha_registro_utc")
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("(NOW() AT TIME ZONE 'UTC')")
            .IsRequired();

        builder.Property(e => e.ModificadoPorUsuario)
            .HasColumnName("modificado_por_usuario")
            .HasMaxLength(150);

        builder.Property(e => e.FechaModificacionUtc)
            .HasColumnName("fecha_modificacion_utc")
            .HasColumnType("timestamptz");

        builder.Property(e => e.ModificacionIp)
            .HasColumnName("modificacion_ip")
            .HasMaxLength(45);

        builder.Property(e => e.ServicioOrigen)
            .HasColumnName("servicio_origen")
            .HasMaxLength(100);

        // Índices
        builder.HasIndex(e => e.GuidFavorito).IsUnique().HasDatabaseName("uq_favoritos_guid");
        builder.HasIndex(e => new { e.GuidClienteRef, e.GuidServicioRef })
            .IsUnique()
            .HasDatabaseName("uq_favoritos_cliente_servicio");
        builder.HasIndex(e => e.GuidClienteRef).HasDatabaseName("idx_favoritos_cliente");
        builder.HasIndex(e => e.GuidServicioRef).HasDatabaseName("idx_favoritos_servicio");
        builder.HasIndex(e => e.Estado).HasDatabaseName("idx_favoritos_estado");

        // Filtro global — soft delete
        builder.HasQueryFilter(e => !e.EsEliminado);
    }
}
