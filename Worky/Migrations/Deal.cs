using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Worky.Migrations;

[Table("Deal")]
[Index("company_id", Name = "deal_ibfk_2")]
[Index("tariff_id", Name = "tarrif_id")]
public partial class Deal
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong tariff_id { get; set; }

    [StringLength(450)] public string? company_id { get; set; }

    public bool status { get; set; }

    public DateOnly date_start { get; set; }

    public DateOnly date_end { get; set; }

    [Column(TypeName = "int(11)")] public int sum { get; set; }

    [ForeignKey("company_id")]
    [InverseProperty("Deals")]
    [JsonIgnore]
    public virtual Company? company { get; set; }

    [ForeignKey("tariff_id")]
    [InverseProperty("Deals")]
    [JsonIgnore]
    public virtual Tarrif tariff { get; set; } = null!;
}