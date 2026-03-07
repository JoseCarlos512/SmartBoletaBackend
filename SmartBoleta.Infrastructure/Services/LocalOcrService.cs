using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Infrastructure.Services;

// Stub implementation. Replace with Tesseract or Azure Cognitive Services.
internal sealed class LocalOcrService : IOcrService
{
    public Task<string> ExtraerTextoAsync(string archivoUrl, CancellationToken cancellationToken = default)
    {
        // TODO: Implement OCR with Tesseract (local) or Azure Cognitive Services (cloud).
        // Tesseract: dotnet add package Tesseract
        // Azure: dotnet add package Azure.AI.Vision.ImageAnalysis
        return Task.FromResult(string.Empty);
    }
}
