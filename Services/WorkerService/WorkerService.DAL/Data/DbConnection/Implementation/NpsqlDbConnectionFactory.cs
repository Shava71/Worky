using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using WorkerService.DAL.Data.DbConnection.Interface;

namespace WorkerService.DAL.Data.DbConnection.Implementation;

public class NpsqlDbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public NpsqlDbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        return new NpgsqlConnection(connectionString);
    }
}