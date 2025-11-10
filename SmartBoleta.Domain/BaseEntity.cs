namespace SmartBoleta.Domain;
public class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
}