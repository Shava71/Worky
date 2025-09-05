using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Manager_filter")]
[Index("manager_id", Name = "manager_filter_ibfk_1")]
[Index("typeOfActivity_id", Name = "typeOfActivity_id")]
public partial class Manager_filter
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong filter_id { get; set; }

    [StringLength(450)]
    public string? manager_id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong typeOfActivity_id { get; set; }

    [ForeignKey("manager_id")]
    [InverseProperty("Manager_filters")]
    public virtual Manager? manager { get; set; }

    [ForeignKey("typeOfActivity_id")]
    [InverseProperty("Manager_filters")]
    public virtual TypeOfActivity typeOfActivity { get; set; } = null!;
}
