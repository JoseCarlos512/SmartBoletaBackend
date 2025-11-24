
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Domain;

public class Usuario : BaseEntity
{
    
    public Usuario() { }
    public Usuario(
        Guid id,
        Guid tenantId,
        string nombre,
        string? correo,
        string? dni,
        byte[] passwordHash,
        byte[] passwordSalt
    ) : base(id)
    {
        TenantId = tenantId;
        Nombre = nombre;
        Correo = correo;
        DNI = dni;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public Guid TenantId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Correo { get; set; }

    public string? DNI { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public bool Estado { get; set; } = true;

    public DateTime CreadoPor { get; set; } = DateTime.UtcNow;

    public DateTime? ActualizadoPor { get; set; }
    public Tenant? Tenant { get; set; }

    public static Usuario Create(
        Guid tenantId,
        string nombre,
        string? correo,
        string? dni,
        byte[] passwordHash,
        byte[] passwordSalt
    )
    {
        return new Usuario(
            Guid.NewGuid(),
            tenantId,
            nombre,
            correo,
            dni,
            passwordHash,
            passwordSalt
        );
    }
}

public static class UsuariosErrors
{
    public static Error NotFound = new
    (
        "Usuarios.NotFound",
        "No existe un usuario con ese ID"
    );
}