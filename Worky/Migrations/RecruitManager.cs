using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("RecruitManager")]
[Index("manager_id", Name = "manager_id")]
[Index("company_id", Name = "recruitmanager_ibfk_2")]
public partial class RecruitManager
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong manager_id { get; set; }

    [StringLength(450)] public string? company_id { get; set; }

    public bool status { get; set; }

    public DateOnly date_start { get; set; }

    public DateOnly date_end { get; set; }

    [Column(TypeName = "int(11)")] public int sum { get; set; }

    [ForeignKey("company_id")]
    [InverseProperty("RecruitManagers")]
    public virtual Company? company { get; set; }
}