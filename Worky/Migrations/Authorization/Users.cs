using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Index("NormalizedEmail", Name = "EmailIndex")]
[Index("NormalizedUserName", Name = "UserNameIndex", IsUnique = true)]
public partial class Users : IdentityUser<string>
{
    public Users()
    {
        Id = Guid.NewGuid().ToString();
    }
    // [Key]
    // [StringLength(450)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string Id { get; set; } = null!;
    //
    // [StringLength(256)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string? UserName { get; set; }
    //
    // [StringLength(256)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string? NormalizedUserName { get; set; }
    //
    // [StringLength(256)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string? Email { get; set; }
    //
    // [StringLength(256)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string? NormalizedEmail { get; set; }
    //
    // [Column(TypeName = "bit(1)")]
    // public ulong EmailConfirmed { get; set; }
    //
    // [Column(TypeName = "text")]
    // public string? PasswordHash { get; set; }
    //
    // [Column(TypeName = "text")]
    // public string? SecurityStamp { get; set; }
    //
    // [Column(TypeName = "text")]
    // public string? ConcurrencyStamp { get; set; }
    //
    // [Column(TypeName = "text")]
    // public string? PhoneNumber { get; set; }
    //
    // [Column(TypeName = "bit(1)")]
    // public ulong PhoneNumberConfirmed { get; set; }
    //
    // [Column(TypeName = "bit(1)")]
    // public ulong TwoFactorEnabled { get; set; }
    //
    // [MaxLength(6)]
    // public DateTime? LockoutEnd { get; set; }
    //
    // [Column(TypeName = "bit(1)")]
    // public ulong LockoutEnabled { get; set; }
    //
    // [Column(TypeName = "int(11)")]
    // public int AccessFailedCount { get; set; }
    public byte[]? image { get; set; }
    [InverseProperty("User")]
    public virtual ICollection<UserClaims> AspNetUserClaims { get; set; } = new List<UserClaims>();

    [InverseProperty("User")]
    public virtual ICollection<UserLogins> AspNetUserLogins { get; set; } = new List<UserLogins>();

    [InverseProperty("User")]
    public virtual ICollection<UserTokens> AspNetUserTokens { get; set; } = new List<UserTokens>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Roles> Roles { get; set; } = new List<Roles>();
    
    [InverseProperty("User")]
    public virtual ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
}
