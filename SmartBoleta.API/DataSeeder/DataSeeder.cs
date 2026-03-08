using Microsoft.EntityFrameworkCore;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions.Security;
using SmartBoleta.Infrastructure;

namespace SmartBoleta.API.DataSeeder;
public static class DataSeeder
{
    public static async Task SeedAdminUsuarioAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartBoletaDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var tenant = await db.Tenants
            .FirstOrDefaultAsync(t => t.Ruc == "20604654093");

        if (tenant == null)
        {
            tenant = Tenant.Create(
                nombreComercial: "SERVIAM SA",
                ruc: "20604654093",
                logoUrl: "www.logo.com/serviam",
                colorPrimario: "red",
                faviconUrl: "www.favicon.com/red"
            );

            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();
        }

        var tenantId = tenant.Id;
        const string correo = "admin@demo.com";

        var existe = await db.Usuarios.AnyAsync(u => u.Correo == correo);
        if (existe) return;

        var salt = hasher.GenerateSalt();
        var hash = hasher.Hash("Admin123!", salt);

        var usuario = Usuario.Create(
            tenantId: tenantId,
            nombre: "Admin Demo",
            correo: correo,
            dni: "00000000",
            rol: Roles.Admin,
            passwordHash: hash,
            passwordSalt: salt
        );

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
    }
}