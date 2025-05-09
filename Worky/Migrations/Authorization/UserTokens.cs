using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[PrimaryKey("UserId", "LoginProvider", "Name")]
public partial class UserTokens : IdentityUserToken<string>
{
    [Key]
    [StringLength(450)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    public string UserId { get; set; } = null!;

    [Key]
    [StringLength(128)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    public string LoginProvider { get; set; } = null!;

    [Key]
    [StringLength(128)]
    [MySqlCharSet("utf8mb3")]
    [MySqlCollation("utf8mb3_uca1400_ai_ci")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Value { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AspNetUserTokens")]
    public virtual Users User { get; set; } = null!;
}
