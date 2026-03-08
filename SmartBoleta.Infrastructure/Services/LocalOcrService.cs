using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartBoleta.Domain.Abstractions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace SmartBoleta.Infrastructure.Services;

/// <summary>
/// Extrae texto de documentos PDF usando PdfPig (funciona para PDFs basados en texto).
/// Para PDFs escaneados (imágenes), integrar Tesseract:
///   dotnet add package Tesseract  (requiere archivos de idioma .traineddata)
/// Para Azure Cognitive Services:
///   dotnet add package Azure.AI.Vision.ImageAnalysis
/// </summary>
internal sealed class LocalOcrService : IOcrService
{
    private readonly string _basePath;
    private readonly ILogger<LocalOcrService> _logger;

    public LocalOcrService(IConfiguration configuration, ILogger<LocalOcrService> logger)
    {
        _basePath = configuration.GetValue<string>("Storage:LocalPath")
            ?? Path.Combine(Directory.GetCurrentDirectory(), "storage");
        _logger = logger;
    }

    public async Task<string> ExtraerTextoAsync(string archivoUrl, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, archivoUrl);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Archivo no encontrado para OCR: {FilePath}", filePath);
            return string.Empty;
        }

        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => await ExtraerTextoPdfAsync(filePath, cancellationToken),
            _ => string.Empty // Para imágenes (PNG, JPG): integrar Tesseract o Azure
        };
    }

    private Task<string> ExtraerTextoPdfAsync(string filePath, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            try
            {
                var sb = new StringBuilder();
                using var document = PdfDocument.Open(filePath);
                foreach (Page page in document.GetPages())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    sb.AppendLine(page.Text);
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al extraer texto del PDF: {FilePath}", filePath);
                return string.Empty;
            }
        }, cancellationToken);
    }
}
