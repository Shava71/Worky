using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WorkerService.DAL.Clients;

public class UserResponse
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [StringLength(256)]
    public string? UserName { get; set; }
    
    [StringLength(256)]
    public string? NormalizedUserName { get; set; }
    
    [StringLength(256)]
    public string? Email { get; set; }
    
    [StringLength(256)]
    public string? NormalizedEmail { get; set; }
    
    public bool EmailConfirmed { get; set; }
    
    public string? PasswordHash { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public bool PhoneNumberConfirmed { get; set; }
    
    public bool TwoFactorEnabled { get; set; }
    
    [MaxLength(6)]
    public DateTime CreatedAt { get; set; }
    
    public int AccessFailedCount { get; set; }
    public byte[]? image { get; set; }
    
}