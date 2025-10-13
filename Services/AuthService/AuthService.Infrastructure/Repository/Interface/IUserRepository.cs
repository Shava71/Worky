using AuthService.Domain.Entities;
using AuthService.Infrastructure.Outbox;

namespace AuthService.Infrastructure.Repository.Interface;

public interface IUserRepository
{
    Task<User> FindByIdAsync(string userId);
    Task CreateUserAsync(User user);
    Task CreateUserWithOutboxMessageAsync(User user, OutboxMessage message);

    Task AddToRoleAsync(User user, Role role);
    Task<User> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<List<string>> GetRolesAsync(User user);
    Task<Role> FindRoleByNameAsync(string roleName);
    Task ExecuteSqlAsync(string sql);
    Task ExecuteSqlWithParamAsync(string sql, object parameters);
    Task<List<OutboxMessage>> GetPendingOutboxAsync(int limit = 50);
    Task MarkOutboxAsSentAsync(Guid outboxId);
}