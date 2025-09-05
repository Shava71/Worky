using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("typeOfActivity")]
[Index("direction", "type", Name = "idx_direction_type")]
public partial class TypeOfActivity
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong id { get; set; }

    [StringLength(100)] public string direction { get; set; } = null!;

    [StringLength(100)] public string type { get; set; } = null!;

    [InverseProperty("typeOfActivity")]
    public virtual ICollection<Manager_filter> Manager_filters { get; set; } = new List<Manager_filter>();

    [InverseProperty("typeOfActivity")]
    public virtual ICollection<Resume_filter> Resume_filters { get; set; } = new List<Resume_filter>();

    [InverseProperty("typeOfActivity")]
    public virtual ICollection<Vacancy_filter> Vacancy_filters { get; set; } = new List<Vacancy_filter>();
}