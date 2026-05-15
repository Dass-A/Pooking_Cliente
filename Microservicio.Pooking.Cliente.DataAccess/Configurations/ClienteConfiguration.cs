using Microservicio.Pooking.Cliente.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Pooking.Cliente.DataAccess.Configurations;

/// <summary>
/// Mapping EF Core para ClienteEntity → tabla booking.cliente.
/// Sin FK física a Auth: UsuarioGuidRef es referencia lógica cross-dominio.
/// </summary>
public class ClienteConfiguration : IEntityTypeConfiguration<ClienteEntity>
{
    public void Configure(EntityTypeBuilder<ClienteEntity> builder)
    {
        // Tabla y esquema
        builder.ToTable("cliente", "booking");

        // [1] Clave primaria
        builder.HasKey(e => e.IdCliente);

        builder.Property(e => e.IdCliente)
            .HasColumnName("id_cliente")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.GuidCliente)
            .HasColumnName("guid_cliente")
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .IsRequired();

        // [2] Referencia lógica al dominio Auth (sin FK física)
        builder.Property(e => e.UsuarioGuidRef)
            .HasColumnName("usuario_guid_ref")
            .HasColumnType("uuid")
            .IsRequired();

        // [3] Datos funcionales
        builder.Property(e => e.Nombres)
            .HasColumnName("nombres")
            .HasMaxLength(200);

        builder.Property(e => e.Apellidos)
            .HasColumnName("apellidos")
            .HasMaxLength(200);

        builder.Property(e => e.RazonSocial)
            .HasColumnName("razon_social")
            .HasMaxLength(200);

        builder.Property(e => e.TipoIdentificacion)
            .HasColumnName("tipo_identificacion")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.NumeroIdentificacion)
            .HasColumnName("numero_identificacion")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Correo)
            .HasColumnName("correo")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(30);

        builder.Property(e => e.Direccion)
            .HasColumnName("direccion")
            .HasMaxLength(500);

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

        // Índices únicos
        builder.HasIndex(e => e.GuidCliente)
            .IsUnique()
            .HasDatabaseName("uq_cliente_guid");

        builder.HasIndex(e => e.UsuarioGuidRef)
            .IsUnique()
            .HasDatabaseName("uq_cliente_usuario_guid_ref");

        builder.HasIndex(e => new { e.TipoIdentificacion, e.NumeroIdentificacion })
            .IsUnique()
            .HasDatabaseName("uq_cliente_numero_id");

        // Índices de consulta
        builder.HasIndex(e => e.Estado).HasDatabaseName("idx_cliente_estado");
        builder.HasIndex(e => e.Correo).HasDatabaseName("idx_cliente_correo");
        builder.HasIndex(e => e.UsuarioGuidRef).HasDatabaseName("idx_cliente_usuario_guid_ref");

        // Filtro global — soft delete
        builder.HasQueryFilter(e => !e.EsEliminado);
    }
}
