using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Index("NormalizedName", Name = "RoleNameIndex", IsUnique = true)]
public partial class Roles : IdentityRole<string>
{
    public Roles()
    {
        Id = Guid.NewGuid().ToString();
    }
    // [Key]
    // [StringLength(450)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string Id { get; set; }

    // [StringLength(256)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string? Name { get; set; }
    //
    // [StringLength(256)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string? NormalizedName { get; set; }
    //
    // [Column(TypeName = "text")]
    // public string? ConcurrencyStamp { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<RoleClaims> AspNetRoleClaims { get; set; } = new List<RoleClaims>();

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<Users> Users { get; set; } = new List<Users>();
    
    [InverseProperty("Role")]
    public virtual ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();

}
