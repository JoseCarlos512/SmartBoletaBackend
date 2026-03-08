using Microsoft.EntityFrameworkCore;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Enums;

namespace SmartBoleta.Infrastructure;

public class SmartBoletaDbContext : DbContext
{
    public SmartBoletaDbContext(DbContextOptions<SmartBoletaDbContext> options) : base(options) { }

    public required DbSet<Tenant> Tenants { get; set; }
    public required DbSet<Usuario> Usuarios { get; set; }
    public required DbSet<Boleta> Boletas { get; set; }
    public required DbSet<CargaMasiva> CargaMasivas { get; set; }
    public required DbSet<CargaMasivaArchivo> CargaMasivaArchivos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(e => e.Id).HasName("PK_Tenants");
            entity.Property(e => e.Id)
                  .HasColumnName("TenantId")
                  .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(e => e.NombreComercial).IsRequired().HasMaxLength(150).HasColumnName("NombreComercial");
            entity.Property(e => e.Ruc).HasMaxLength(20).HasColumnName("RUC");
            entity.Property(e => e.LogoUrl).HasMaxLength(500).HasColumnName("LogoUrl");
            entity.Property(e => e.ColorPrimario).HasMaxLength(20).HasColumnName("ColorPrimario");
            entity.Property(e => e.FaviconUrl).HasMaxLength(500).HasColumnName("FaviconUrl");

            entity.Property(e => e.Estado)
                  .HasConversion<byte>()
                  .HasColumnType("TINYINT")
                  .HasDefaultValue((byte)1)
                  .HasColumnName("Estado");

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
            entity.HasKey(e => e.Id).HasName("PK_Usuarios");
            entity.Property(e => e.Id)
                  .HasColumnName("UsuarioId")
                  .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(e => e.TenantId).IsRequired().HasColumnName("TenantId");
            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Usuarios)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_Usuarios_Tenants");

            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150).HasColumnName("Nombre");
            entity.Property(e => e.Correo).HasMaxLength(150).HasColumnName("Correo");
            entity.Property(e => e.DNI).HasMaxLength(20).HasColumnName("DNI");
            entity.Property(e => e.Rol).IsRequired().HasMaxLength(20).HasDefaultValue(Domain.Roles.User).HasColumnName("Rol");

            entity.Property(e => e.PasswordHash).IsRequired().HasColumnType("VARBINARY(MAX)").HasColumnName("PasswordHash");
            entity.Property(e => e.PasswordSalt).IsRequired().HasColumnType("VARBINARY(MAX)").HasColumnName("PasswordSalt");

            entity.Property(e => e.Estado)
                  .HasConversion<byte>()
                  .HasColumnType("TINYINT")
                  .HasDefaultValue((byte)1)
                  .HasColumnName("Estado");

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

        modelBuilder.Entity<Boleta>(entity =>
        {
            entity.ToTable("Boletas");
            entity.HasKey(e => e.Id).HasName("PK_Boletas");
            entity.Property(e => e.Id)
                  .HasColumnName("BoletaId")
                  .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(e => e.TenantId).IsRequired().HasColumnName("TenantId");
            entity.HasOne(e => e.Tenant)
                  .WithMany()
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_Boletas_Tenants");

            entity.Property(e => e.UsuarioId).IsRequired().HasColumnName("UsuarioId");
            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.Boletas)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_Boletas_Usuarios");

            entity.Property(e => e.Periodo).IsRequired().HasMaxLength(10).HasColumnName("Periodo");
            entity.Property(e => e.ArchivoNombre).IsRequired().HasMaxLength(255).HasColumnName("ArchivoNombre");
            entity.Property(e => e.ArchivoUrl).IsRequired().HasMaxLength(500).HasColumnName("ArchivoUrl");

            entity.Property(e => e.Estado)
                  .HasConversion<int>()
                  .HasColumnName("Estado");

            entity.Property(e => e.TextoOcr).HasColumnType("NVARCHAR(MAX)").HasColumnName("TextoOcr").IsRequired(false);

            entity.Property(e => e.FechaSubida)
                  .HasColumnType("DATETIME2")
                  .HasDefaultValueSql("SYSDATETIME()")
                  .HasColumnName("FechaSubida");

            entity.Property(e => e.FechaFirma)
                  .HasColumnType("DATETIME2")
                  .HasColumnName("FechaFirma")
                  .IsRequired(false);
        });

        modelBuilder.Entity<CargaMasiva>(entity =>
        {
            entity.ToTable("CargaMasivas");
            entity.HasKey(e => e.Id).HasName("PK_CargaMasivas");
            entity.Property(e => e.Id)
                  .HasColumnName("CargaMasivaId")
                  .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(e => e.TenantId).IsRequired().HasColumnName("TenantId");
            entity.HasOne(e => e.Tenant)
                  .WithMany()
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_CargaMasivas_Tenants");

            entity.Property(e => e.UsuarioSolicitanteId).IsRequired().HasColumnName("UsuarioSolicitanteId");
            entity.HasOne(e => e.UsuarioSolicitante)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioSolicitanteId)
                  .OnDelete(DeleteBehavior.NoAction)
                  .HasConstraintName("FK_CargaMasivas_Usuarios");

            entity.Property(e => e.Periodo).IsRequired().HasMaxLength(10).HasColumnName("Periodo");

            entity.Property(e => e.Estado)
                  .HasConversion<int>()
                  .HasColumnName("Estado");

            entity.Property(e => e.TotalArchivos).HasColumnName("TotalArchivos");
            entity.Property(e => e.ArchivosProcessados).HasColumnName("ArchivosProcessados");
            entity.Property(e => e.ArchivosExitosos).HasColumnName("ArchivosExitosos");
            entity.Property(e => e.ArchivosFallidos).HasColumnName("ArchivosFallidos");

            entity.Property(e => e.FechaInicio)
                  .HasColumnType("DATETIME2")
                  .HasDefaultValueSql("SYSDATETIME()")
                  .HasColumnName("FechaInicio");

            entity.Property(e => e.FechaFin)
                  .HasColumnType("DATETIME2")
                  .HasColumnName("FechaFin")
                  .IsRequired(false);

            entity.HasMany(e => e.Archivos)
                  .WithOne(a => a.CargaMasiva)
                  .HasForeignKey(a => a.CargaMasivaId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_CargaMasivaArchivos_CargaMasivas");
        });

        modelBuilder.Entity<CargaMasivaArchivo>(entity =>
        {
            entity.ToTable("CargaMasivaArchivos");
            entity.HasKey(e => e.Id).HasName("PK_CargaMasivaArchivos");
            entity.Property(e => e.Id)
                  .HasColumnName("CargaMasivaArchivoId")
                  .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(e => e.CargaMasivaId).IsRequired().HasColumnName("CargaMasivaId");
            entity.Property(e => e.ArchivoNombre).IsRequired().HasMaxLength(255).HasColumnName("ArchivoNombre");
            entity.Property(e => e.ArchivoUrl).IsRequired().HasMaxLength(500).HasColumnName("ArchivoUrl");
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100).HasColumnName("ContentType");

            entity.Property(e => e.Estado)
                  .HasConversion<int>()
                  .HasColumnName("Estado");

            entity.Property(e => e.UsuarioIdentificadoId).HasColumnName("UsuarioIdentificadoId").IsRequired(false);
            entity.Property(e => e.BoletaId).HasColumnName("BoletaId").IsRequired(false);
            entity.Property(e => e.ErrorMensaje).HasMaxLength(500).HasColumnName("ErrorMensaje").IsRequired(false);
            entity.Property(e => e.TextoOcr).HasColumnType("NVARCHAR(MAX)").HasColumnName("TextoOcr").IsRequired(false);
        });
    }
}
