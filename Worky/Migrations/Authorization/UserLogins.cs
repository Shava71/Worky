using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[PrimaryKey("LoginProvider", "ProviderKey")]
[Index("UserId", Name = "IX_AspNetUserLogins_UserId")]
public partial class UserLogins : IdentityUserLogin<string>
{
    [Key]
    [StringLength(128)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    public string LoginProvider { get; set; } = null!;

    [Key]
    [StringLength(128)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    public string ProviderKey { get; set; } = null!;

    [Column(TypeName = "text")] public string? ProviderDisplayName { get; set; }

    [StringLength(450)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("AspNetUserLogins")]
    public virtual Users User { get; set; } = null!;
}