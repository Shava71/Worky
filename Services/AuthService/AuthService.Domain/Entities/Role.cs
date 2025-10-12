namespace AuthService.Domain.Entities;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    
    public ICollection<UserRole> Users { get; set; } = new List<UserRole>();
}