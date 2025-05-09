using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Manager")]
public partial class Manager
{
    [Key]
    [StringLength(450)]
    public string id { get; set; } = null!;

    [StringLength(100)]
    public string second_name { get; set; } = null!;

    [StringLength(50)]
    public string first_name { get; set; } = null!;

    [StringLength(50)]
    public string surname { get; set; } = null!;

    [Column(TypeName = "smallint(6)")]
    public short age { get; set; }

    [Column(TypeName = "text")]
    public string? description { get; set; }

    [Column(TypeName = "int(11)")]
    public int? price { get; set; }

    [InverseProperty("manager")]
    public virtual ICollection<Manager_filter> Manager_filters { get; set; } = new List<Manager_filter>();
}
