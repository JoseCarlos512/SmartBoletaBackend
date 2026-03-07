using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartBoleta.Domain.Abstractions;

namespace SmartBoleta.Infrastructure;

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServerDatabase")!;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
