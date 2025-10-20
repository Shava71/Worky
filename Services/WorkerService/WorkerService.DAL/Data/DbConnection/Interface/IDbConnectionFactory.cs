using System.Data;

namespace WorkerService.DAL.Data.DbConnection.Interface;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}