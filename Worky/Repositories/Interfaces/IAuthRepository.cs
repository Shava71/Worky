using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Worky.Migrations;

namespace Worky.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<Users> FindByIdAsync(string userId);
    Task<IdentityResult> CreateUserAsync(Users user, string password);
    Task AddToRoleAsync(Users user, string roleName);
    Task<Users> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(Users user, string password);
    Task<IList<string>> GetRolesAsync(Users user);
    Task<Roles> FindRoleByNameAsync(string roleName);
    Task ExecuteSqlAsync(string sql);
    Task ExecuteSqlWithParamAsync(string sql, object parameters);
}