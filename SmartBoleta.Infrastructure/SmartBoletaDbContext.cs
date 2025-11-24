using Microsoft.EntityFrameworkCore;
using SmartBoleta.Domain;
using System;

namespace SmartBoleta.Infrastructure
{
    public class SmartBoletaDbContext : DbContext
    {
        public SmartBoletaDbContext(DbContextOptions<SmartBoletaDbContext> options) : base(options) { }

        public required DbSet<Tenant> Tenants { get; set; }
        public required DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenants");

                // PK -> TenantId
                entity.HasKey(e => e.Id).HasName("PK_Tenants");
                entity.Property(e => e.Id)
                      .HasColumnName("TenantId")
                      .HasDefaultValueSql("NEWSEQUENTIALID()");

                // NombreComercial NVARCHAR(150) NOT NULL
                entity.Property(e => e.NombreComercial)
                      .IsRequired()
                      .HasMaxLength(150)
                      .HasColumnName("NombreComercial");

                // RUC NVARCHAR(20) NULL
                entity.Property(e => e.Ruc)
                      .HasMaxLength(20)
                      .HasColumnName("RUC");

                // LogoUrl NVARCHAR(500) NULL
                entity.Property(e => e.LogoUrl)
                      .HasMaxLength(500)
                      .HasColumnName("LogoUrl");

                // ColorPrimario NVARCHAR(20) NULL
                entity.Property(e => e.ColorPrimario)
                      .HasMaxLength(20)
                      .HasColumnName("ColorPrimario");

                // FaviconUrl NVARCHAR(500) NULL
                entity.Property(e => e.FaviconUrl)
                      .HasMaxLength(500)
                      .HasColumnName("FaviconUrl");

                // Estado TINYINT NOT NULL DEFAULT 1 (map boolean <-> tinyint)
                entity.Property(e => e.Estado)
                      .HasConversion<byte>()
                      .HasColumnType("TINYINT")
                      .HasDefaultValue((byte)1)
                      .HasColumnName("Estado");

                // CreatedAt / UpdatedAt: se configuran como propiedades (sombras si no existen en la entidad)
                entity.Property<DateTime>("CreatedAt")
                      .HasColumnType("DATETIME2")
                      .HasDefaultValueSql("SYSDATETIME()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property<DateTime?>("UpdatedAt")
                      .HasColumnType("DATETIME2")
                      .HasColumnName("UpdatedAt")
                      .IsRequired(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");

                // PK -> UsuarioId
                entity.HasKey(e => e.Id).HasName("PK_Usuarios");
                entity.Property(e => e.Id)
                      .HasColumnName("UsuarioId")
                      .HasDefaultValueSql("NEWSEQUENTIALID()");

                // TenantId FK
                entity.Property(e => e.TenantId)
                      .IsRequired()
                      .HasColumnName("TenantId");

                entity.HasOne(e => e.Tenant)
                      .WithMany()
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_Usuarios_Tenants");

                // Nombre NVARCHAR(150) NOT NULL
                entity.Property(e => e.Nombre)
                      .IsRequired()
                      .HasMaxLength(150)
                      .HasColumnName("Nombre");

                // Correo NVARCHAR(150) NULL
                entity.Property(e => e.Correo)
                      .HasMaxLength(150)
                      .HasColumnName("Correo");

                // DNI NVARCHAR(20) NULL
                entity.Property(e => e.DNI)
                      .HasMaxLength(20)
                      .HasColumnName("DNI");

                // PasswordHash / PasswordSalt VARBINARY(MAX) NOT NULL
                entity.Property(e => e.PasswordHash)
                      .IsRequired()
                      .HasColumnType("VARBINARY(MAX)")
                      .HasColumnName("PasswordHash");

                entity.Property(e => e.PasswordSalt)
                      .IsRequired()
                      .HasColumnType("VARBINARY(MAX)")
                      .HasColumnName("PasswordSalt");

                // Estado TINYINT NOT NULL DEFAULT 1 (map boolean <-> tinyint)
                entity.Property(e => e.Estado)
                      .HasConversion<byte>()
                      .HasColumnType("TINYINT")
                      .HasDefaultValue((byte)1)
                      .HasColumnName("Estado");

                // CreadoPor / ActualizadoPor (fechas)
                entity.Property(e => e.CreadoPor)
                      .HasColumnType("DATETIME2")
                      .HasColumnName("CreadoPor")
                      .HasDefaultValueSql("SYSDATETIME()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ActualizadoPor)
                      .HasColumnType("DATETIME2")
                      .HasColumnName("ActualizadoPor")
                      .IsRequired(false);
            });
        }
    }
}
