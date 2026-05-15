using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Pooking.Cliente.DataAccess.Configurations;

/// <summary>
/// Mapping EF Core para ReservasEntity → tabla booking.reservas.
/// FK física al dominio Cliente. Snapshot inmutable del servicio.
/// </summary>
public class ReservasConfiguration : IEntityTypeConfiguration<ReservasEntity>
{
    public void Configure(EntityTypeBuilder<ReservasEntity> builder)
    {
        builder.ToTable("reservas", "booking");

        // [1] PK
        builder.HasKey(e => e.IdReserva);

        builder.Property(e => e.IdReserva)
            .HasColumnName("id_reserva")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.GuidReserva)
            .HasColumnName("guid_reserva")
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        // [2] FK al dominio Cliente
        builder.Property(e => e.IdCliente)
            .HasColumnName("id_cliente")
            .IsRequired();

        // [3] Referencia lógica al dominio Servicio
        builder.Property(e => e.GuidServicioRef)
            .HasColumnName("guid_servicio_ref")
            .HasColumnType("uuid")
            .IsRequired();

        // [4] Snapshot inmutable
        builder.Property(e => e.NombreServicioSnap)
            .HasColumnName("nombre_servicio_snap")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.TipoServicioSnap)
            .HasColumnName("tipo_servicio_snap")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(e => e.NombreProveedor)
            .HasColumnName("nombre_proveedor")
            .HasMaxLength(200)
            .IsRequired();

        // [5] Identificador externo
        builder.Property(e => e.IdReservaExterna)
            .HasColumnName("id_reserva_externa")
            .HasMaxLength(100)
            .IsRequired();

        // [6] Datos funcionales
        builder.Property(e => e.FechaReservaUtc)
            .HasColumnName("fecha_reserva_utc")
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("(NOW() AT TIME ZONE 'UTC')")
            .IsRequired();

        builder.Property(e => e.FechaInicio)
            .HasColumnName("fecha_inicio")
            .HasColumnType("timestamp(0)")
            .IsRequired();

        builder.Property(e => e.FechaFin)
            .HasColumnName("fecha_fin")
            .HasColumnType("timestamp(0)");

        builder.Property(e => e.CanalOrigen)
            .HasColumnName("canal_origen")
            .HasMaxLength(50);

        builder.Property(e => e.MontoTotal)
            .HasColumnName("monto_total")
            .HasColumnType("numeric(12,2)")
            .HasDefaultValue(0.00m)
            .IsRequired();

        builder.Property(e => e.Moneda)
            .HasColumnName("moneda")
            .HasColumnType("char(3)")
            .HasDefaultValue("USD")
            .IsRequired();

        builder.Property(e => e.Observaciones)
            .HasColumnName("observaciones")
            .HasMaxLength(500);

        // [7] Estado y ciclo de vida
        builder.Property(e => e.Estado)
            .HasColumnName("estado")
            .HasColumnType("char(4)")
            .HasDefaultValue("CONF")
            .IsRequired();

        builder.Property(e => e.MotivoCancelacion)
            .HasColumnName("motivo_cancelacion")
            .HasMaxLength(300);

        builder.Property(e => e.FechaCancelacionUtc)
            .HasColumnName("fecha_cancelacion_utc")
            .HasColumnType("timestamptz");

        builder.Property(e => e.EsEliminado)
            .HasColumnName("es_eliminado")
            .HasDefaultValue(false)
            .IsRequired();

        // [8] Auditoría
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
        builder.HasIndex(e => e.GuidReserva).IsUnique().HasDatabaseName("uq_reservas_guid");
        builder.HasIndex(e => e.IdCliente).HasDatabaseName("idx_reservas_cliente");
        builder.HasIndex(e => e.GuidServicioRef).HasDatabaseName("idx_reservas_guid_servicio");
        builder.HasIndex(e => e.Estado).HasDatabaseName("idx_reservas_estado");
        builder.HasIndex(e => e.FechaReservaUtc).HasDatabaseName("idx_reservas_fecha_reserva");
        builder.HasIndex(e => e.FechaInicio).HasDatabaseName("idx_reservas_fecha_inicio");
        builder.HasIndex(e => e.IdReservaExterna).HasDatabaseName("idx_reservas_id_externa");

        // Relación con Cliente (FK interna al mismo dominio)
        builder.HasOne(e => e.Cliente)
            .WithMany()
            .HasForeignKey(e => e.IdCliente)
            .HasConstraintName("fk_reservas_cliente")
            .OnDelete(DeleteBehavior.NoAction);

        // Filtro global — soft delete
        builder.HasQueryFilter(e => !e.EsEliminado);
    }
}
