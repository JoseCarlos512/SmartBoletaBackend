using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBoleta.Domain;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Infrastructure.BackgroundJobs;

public class CargaMasivaBackgroundJob
{
    private readonly SmartBoletaDbContext _dbContext;
    private readonly IOcrService _ocrService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CargaMasivaBackgroundJob> _logger;

    // Patrones de DNI peruano (8 dígitos)
    private static readonly Regex DniRegex = new(
        @"(?:DNI|D\.N\.I\.?|Nro\.?\s*Doc\.?)[:\s]*(\d{8})\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    // Patrón alternativo: número de 8 dígitos solo (menos específico)
    private static readonly Regex DniSimpleRegex = new(
        @"\b(\d{8})\b",
        RegexOptions.Compiled
    );

    public CargaMasivaBackgroundJob(
        SmartBoletaDbContext dbContext,
        IOcrService ocrService,
        INotificationService notificationService,
        ILogger<CargaMasivaBackgroundJob> logger)
    {
        _dbContext = dbContext;
        _ocrService = ocrService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task ProcesarAsync(Guid cargaMasivaId)
    {
        var cargaMasiva = await _dbContext.CargaMasivas
            .Include(c => c.Archivos)
            .FirstOrDefaultAsync(c => c.Id == cargaMasivaId);

        if (cargaMasiva is null)
        {
            _logger.LogWarning("CargaMasiva no encontrada: {CargaMasivaId}", cargaMasivaId);
            return;
        }

        _logger.LogInformation("Iniciando carga masiva {CargaMasivaId} con {Total} archivos",
            cargaMasivaId, cargaMasiva.Archivos.Count);

        cargaMasiva.IniciarProcesamiento(cargaMasiva.Archivos.Count);
        await _dbContext.SaveChangesAsync();

        // Cargar todos los usuarios activos del tenant para identificación
        var usuarios = await _dbContext.Usuarios
            .Where(u => u.TenantId == cargaMasiva.TenantId && u.Estado)
            .ToListAsync();

        foreach (var archivo in cargaMasiva.Archivos)
        {
            archivo.MarcarProcesando();
            await _dbContext.SaveChangesAsync();

            try
            {
                var textoOcr = await _ocrService.ExtraerTextoAsync(archivo.ArchivoUrl);

                _logger.LogInformation("OCR completado para {Archivo}: {Chars} caracteres extraídos",
                    archivo.ArchivoNombre, textoOcr?.Length ?? 0);

                var usuario = IdentificarUsuario(textoOcr ?? string.Empty, usuarios);

                if (usuario is not null)
                {
                    var boleta = Boleta.Create(
                        cargaMasiva.TenantId,
                        usuario.Id,
                        cargaMasiva.Periodo,
                        archivo.ArchivoNombre,
                        archivo.ArchivoUrl
                    );
                    boleta.ActualizarOcr(textoOcr ?? string.Empty);

                    _dbContext.Boletas.Add(boleta);
                    await _dbContext.SaveChangesAsync();

                    archivo.MarcarExitoso(usuario.Id, boleta.Id, textoOcr ?? string.Empty);
                    cargaMasiva.RegistrarResultadoArchivo(true);

                    await _notificationService.NotificarUsuarioAsync(
                        usuario.Id,
                        "boleta_cargada",
                        new
                        {
                            boletaId = boleta.Id,
                            periodo = boleta.Periodo,
                            archivoNombre = archivo.ArchivoNombre
                        }
                    );

                    _logger.LogInformation("Boleta creada para usuario {UsuarioId} desde {Archivo}",
                        usuario.Id, archivo.ArchivoNombre);
                }
                else
                {
                    var error = string.IsNullOrWhiteSpace(textoOcr)
                        ? "No se pudo extraer texto del documento (PDF escaneado o formato no soportado)"
                        : "No se encontró un usuario registrado con el DNI o nombre del documento";

                    archivo.MarcarFallido(error);
                    cargaMasiva.RegistrarResultadoArchivo(false);

                    _logger.LogWarning("No se identificó usuario para {Archivo}", archivo.ArchivoNombre);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando archivo {Archivo}", archivo.ArchivoNombre);
                archivo.MarcarFallido($"Error inesperado: {ex.Message}");
                cargaMasiva.RegistrarResultadoArchivo(false);
            }

            await _dbContext.SaveChangesAsync();
        }

        cargaMasiva.Completar();
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Carga masiva {CargaMasivaId} completada. Exitosos: {Exitosos}, Fallidos: {Fallidos}",
            cargaMasivaId, cargaMasiva.ArchivosExitosos, cargaMasiva.ArchivosFallidos);

        await _notificationService.NotificarUsuarioAsync(
            cargaMasiva.UsuarioSolicitanteId,
            "carga_masiva_completada",
            new
            {
                cargaMasivaId = cargaMasiva.Id,
                estado = cargaMasiva.Estado.ToString(),
                totalArchivos = cargaMasiva.TotalArchivos,
                archivosExitosos = cargaMasiva.ArchivosExitosos,
                archivosFallidos = cargaMasiva.ArchivosFallidos,
                periodo = cargaMasiva.Periodo,
                fechaFin = cargaMasiva.FechaFin
            }
        );
    }

    private static Usuario? IdentificarUsuario(string textoOcr, List<Usuario> usuarios)
    {
        if (string.IsNullOrWhiteSpace(textoOcr)) return null;

        // 1. Intentar con patrón explícito DNI: XXXXXXXX
        var match = DniRegex.Match(textoOcr);
        if (match.Success)
        {
            var dni = match.Groups[1].Value;
            var usuario = usuarios.FirstOrDefault(u =>
                !string.IsNullOrWhiteSpace(u.DNI) && u.DNI == dni);
            if (usuario is not null) return usuario;
        }

        // 2. Buscar cualquier secuencia de 8 dígitos y compararla con DNIs registrados
        var dniMatches = DniSimpleRegex.Matches(textoOcr);
        foreach (Match m in dniMatches)
        {
            var candidato = m.Groups[1].Value;
            var usuario = usuarios.FirstOrDefault(u =>
                !string.IsNullOrWhiteSpace(u.DNI) && u.DNI == candidato);
            if (usuario is not null) return usuario;
        }

        // 3. Buscar por nombre completo del usuario en el texto
        foreach (var usuario in usuarios)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nombre)) continue;

            // Comparar nombre completo normalizado
            var nombreNormalizado = NormalizarTexto(usuario.Nombre);
            var textoNormalizado = NormalizarTexto(textoOcr);

            if (textoNormalizado.Contains(nombreNormalizado, StringComparison.Ordinal))
                return usuario;
        }

        return null;
    }

    private static string NormalizarTexto(string texto)
        => texto.ToUpperInvariant()
                .Replace("Á", "A").Replace("É", "E").Replace("Í", "I")
                .Replace("Ó", "O").Replace("Ú", "U").Replace("Ñ", "N");
}
