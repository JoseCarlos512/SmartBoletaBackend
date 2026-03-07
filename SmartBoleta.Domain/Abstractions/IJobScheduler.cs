namespace SmartBoleta.Domain.Abstractions;

public interface IJobScheduler
{
    void EnqueueOcrJob(Guid boletaId);
}
