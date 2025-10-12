using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Domain.Entities;

public class User
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int AccessFailedCount { get; set; }
    public byte[]? image { get; set; }
    
    public ICollection<UserRole>? Roles { get; set; } = new List<UserRole>();

    public User(){}
    public User(string _userName, string _email, string _passwordHash, string _phoneNumber)
    {
        UserName = _userName;
        Email = _email;
        NormalizedEmail = _email.ToUpper();
        EmailConfirmed = false;
        PasswordHash = CreatePasswordHash(_passwordHash);
        PhoneNumber = _phoneNumber;
        PhoneNumberConfirmed = false;
        TwoFactorEnabled = false;
    }
    

    public void AddRole(Role role)
    {
        if (Roles.Any(r => r.RoleId == role.Id))
        {
            return;
        }
        Roles.Add(new UserRole(this, role));
    }

    public void SetPasswordHash(string password)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public string CreatePasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool CheckPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }
}