namespace SmartBoleta.Domain.Abstractions;

public interface IStorageService
{
    Task<string> SubirArchivoAsync(Stream contenido, string nombreArchivo, string contentType, CancellationToken cancellationToken = default);
    Task EliminarArchivoAsync(string archivoUrl, CancellationToken cancellationToken = default);
}
