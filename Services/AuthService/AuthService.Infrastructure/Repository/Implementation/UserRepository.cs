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

    public async Task<IList<string>> GetRolesAsync(User user)
    {
        List<UserRole> UserRoles = user.Roles.ToList();
        List<string> RoleNames = await _dbContext.Role.Where(r => UserRoles.Any(ur => ur.RoleId == r.Id)).Select(r => r.Name).ToListAsync();
        return RoleNames;
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