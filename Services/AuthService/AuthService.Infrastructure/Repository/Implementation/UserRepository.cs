using System.Data;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repository.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AuthService.Infrastructure.Repository.Implementation;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _dbContext;
    private readonly string _connectionString;

    public UserRepository(AuthDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<User?> FindByIdAsync(string userId)
    {
        Guid GuidUser = Guid.Parse(userId);
        return await _dbContext.User.Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == GuidUser);
    }
    
    public async Task CreateUserAsync(User user)
    {
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddToRoleAsync(User user, Role role)
    {
        user.AddRole(role);
        UserRole newUserRole = new UserRole(user, role);
        await _dbContext.UserRole.AddAsync(newUserRole);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _dbContext.User.Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper());
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return user.CheckPassword(password);
    }

    public async Task<List<string>> GetRolesAsync(User user)
    {
        List<Guid> roleIds = user.Roles.Select(ur => ur.RoleId).ToList();

        List<string> roleNames = await _dbContext.Role
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync();

        return roleNames;
    }

    public async Task<Role?> FindRoleByNameAsync(string roleName)
    {
        return await _dbContext.Role.FirstOrDefaultAsync(r => r.Name == roleName);
    }

    public async Task ExecuteSqlAsync(string sql)
    {
        using (IDbConnection db = new NpgsqlConnection(_connectionString))
        {
            await db.ExecuteAsync(sql);
        }
    }

    public async Task ExecuteSqlWithParamAsync(string sql, object parameters)
    {
        if (parameters == null) throw new ArgumentNullException(nameof(parameters));
        using (IDbConnection db = new NpgsqlConnection(_connectionString))
        {
            await db.ExecuteAsync(sql, parameters);
        }
    }
}