using System.Data;

namespace CompanyService.DAL.Data.DbConnection.Interface;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}