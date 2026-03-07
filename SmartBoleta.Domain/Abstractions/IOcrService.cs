namespace SmartBoleta.Domain.Abstractions;

public interface IOcrService
{
    Task<string> ExtraerTextoAsync(string archivoUrl, CancellationToken cancellationToken = default);
}
