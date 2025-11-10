using Microsoft.EntityFrameworkCore;
using SmartBoleta.Domain;

namespace SmartBoleta.Infrastructure;

public class SmartBoletaDbContext(DbContextOptions<SmartBoletaDbContext> options) : DbContext(options)
{
    public required DbSet<Tenant> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreComercial).IsRequired().HasMaxLength(200);
            // Additional configurations can be added here
        });
    }

}
