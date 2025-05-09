using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Tarrif")]
public partial class Tarrif
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong id { get; set; }

    [StringLength(100)]
    public string name { get; set; } = null!;

    [Column(TypeName = "int(11)")]
    public int price { get; set; }

    [Column(TypeName = "text")]
    public string? description { get; set; }
    [Column(TypeName = "int")]
    public int vacancy_count { get; set; }

    [InverseProperty("tariff")]
    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
}
