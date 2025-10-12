namespace AuthService.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; }

    public UserRole(User user, Role role)
    {
        User = user;
        UserId = user.Id;
        
        Role = role;
        RoleId = role.Id;
    }
}