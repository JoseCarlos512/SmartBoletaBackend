using System.Data;

namespace SmartBoleta.Domain.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
