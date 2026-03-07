
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
        string rol,
        byte[] passwordHash,
        byte[] passwordSalt
    ) : base(id)
    {
        TenantId = tenantId;
        Nombre = nombre;
        Correo = correo;
        DNI = dni;
        Rol = rol;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public Guid TenantId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Correo { get; set; }
    public string? DNI { get; set; }
    public string Rol { get; set; } = Roles.User;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public bool Estado { get; set; } = true;
    public DateTime CreadoPor { get; set; } = DateTime.UtcNow;
    public DateTime? ActualizadoPor { get; set; }
    public Tenant? Tenant { get; set; }
    public ICollection<Boleta>? Boletas { get; set; }

    public static Usuario Create(
        Guid tenantId,
        string nombre,
        string? correo,
        string? dni,
        string rol,
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
            rol,
            passwordHash,
            passwordSalt
        );
    }
}

public static class UsuariosErrors
{
    public static readonly Error NotFound = new("Usuarios.NotFound", "No existe un usuario con ese ID");
    public static readonly Error InvalidCredentials = new("Usuarios.InvalidCredentials", "Correo o contraseña incorrectos");
}