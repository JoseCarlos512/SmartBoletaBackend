using Microsoft.Extensions.Configuration;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Infrastructure.Services;

internal sealed class LocalStorageService : IStorageService
{
    private readonly string _basePath;

    public LocalStorageService(IConfiguration configuration)
    {
        _basePath = configuration.GetValue<string>("Storage:LocalPath")
            ?? Path.Combine(Directory.GetCurrentDirectory(), "storage");

        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SubirArchivoAsync(Stream contenido, string nombreArchivo, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueName = $"{Guid.NewGuid()}_{nombreArchivo}";
        var filePath = Path.Combine(_basePath, uniqueName);

        await using var file = File.Create(filePath);
        await contenido.CopyToAsync(file, cancellationToken);

        return uniqueName;
    }

    public Task EliminarArchivoAsync(string archivoUrl, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, archivoUrl);
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}
