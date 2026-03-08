namespace SmartBoleta.Domain.Abstractions;

public interface IJobScheduler
{
    void EnqueueOcrJob(Guid boletaId);
    void EnqueueCargaMasivaJob(Guid cargaMasivaId);
}
