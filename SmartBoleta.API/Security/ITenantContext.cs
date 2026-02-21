namespace SmartBoleta.API.Security;

public interface ITenantContext
{
    Guid? GetTenantId();
}
