using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Worky.Context;
using Worky.Migrations;
using Worky.Repositories.Interfaces;

namespace Worky.Repositories.Implementations;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<Users> _userManager;
    private readonly RoleManager<Roles> _roleManager;
    private readonly WorkyDbContext _dbContext;
    private readonly string _connectionString;

    public AuthRepository(UserManager<Users> userManager, RoleManager<Roles> roleManager, WorkyDbContext dbContext, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<Users> FindByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> CreateUserAsync(Users user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task AddToRoleAsync(Users user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<Users> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<bool> CheckPasswordAsync(Users user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IList<string>> GetRolesAsync(Users user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<Roles> FindRoleByNameAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task ExecuteSqlAsync(string sql)
    {
        using (IDbConnection db = new MySqlConnection(_connectionString))
        {
            await db.ExecuteAsync(sql);
        }
    }

    public async Task ExecuteSqlWithParamAsync(string sql, object parameters)
    {
        if (parameters == null) throw new ArgumentNullException(nameof(parameters));
        using (IDbConnection db = new MySqlConnection(_connectionString))
        {
            await db.ExecuteAsync(sql, parameters);
        }
    }
}