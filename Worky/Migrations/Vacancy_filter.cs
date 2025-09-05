using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Vacancy_filter")]
[Index("typeOfActivity_id", Name = "typeOfActivity_id")]
[Index("vacancy_id", Name = "vacancy_id")]
public partial class Vacancy_filter
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong filter_id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong vacancy_id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong typeOfActivity_id { get; set; }

    [ForeignKey("typeOfActivity_id")]
    [InverseProperty("Vacancy_filters")]
    public virtual TypeOfActivity typeOfActivity { get; set; } = null!;

    [ForeignKey("vacancy_id")]
    [InverseProperty("Vacancy_filters")]
    public virtual Vacancy vacancy { get; set; } = null!;
}
