using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Index("UserId", Name = "IX_AspNetUserClaims_UserId")]
public partial class UserClaims : IdentityUserClaim<string>
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [StringLength(450)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    public string UserId { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? ClaimType { get; set; }

    [Column(TypeName = "text")]
    public string? ClaimValue { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AspNetUserClaims")]
    public virtual Users User { get; set; } = null!;
}
