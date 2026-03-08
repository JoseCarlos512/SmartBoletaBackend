namespace SmartBoleta.API.Controllers.Request;

public class SubirBoletasMasivasRequest
{
    public string Periodo { get; set; } = null!;
    public List<IFormFile> Archivos { get; set; } = [];
}
