using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[PrimaryKey("UserId", "RoleId")]
[Index("RoleId", Name = "IX_AspNetUserRoles_RoleId")]
public partial class UserRoles : IdentityUserRole<string>
{
    // [Key]
    // [StringLength(450)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string UserId { get; set; } = null!;
    //
    // [Key]
    // [StringLength(450)]
    // [MySqlCharSet("utf8mb3")]
    // [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    // public string RoleId { get; set; } = null!;

    [ForeignKey("UserId")] public virtual Users User { get; set; } = null!;

    [ForeignKey("RoleId")] public virtual Roles Role { get; set; } = null!;
}