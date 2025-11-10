namespace SmartBoleta.Domain;
public class BaseEntity
{
    protected BaseEntity(){}

    protected BaseEntity(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; init; }
}