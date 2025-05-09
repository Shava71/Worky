using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Vacancy")]
[Index("education_id", Name = "education_id")]
[Index("income_date", Name = "income_date")]
[Index("min_salary", Name = "min_salary")]
[Index("company_id", Name = "vacancy_ibfk_1")]
public partial class Vacancy
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong id { get; set; }

    [StringLength(450)]
    public string? company_id { get; set; }

    [StringLength(100)]
    public string post { get; set; } = null!;

    [Column(TypeName = "int(11)")]
    public int min_salary { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong education_id { get; set; }

    [Column(TypeName = "smallint(6)")]
    public short? experience { get; set; }

    [Column(TypeName = "text")]
    public string? description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime income_date { get; set; }

    [Column(TypeName = "int(11)")]
    public int? max_salary { get; set; }

    [InverseProperty("vacancy")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("vacancy")]
    public virtual ICollection<Vacancy_filter> Vacancy_filters { get; set; } = new List<Vacancy_filter>();

    [ForeignKey("company_id")]
    [InverseProperty("Vacancies")]
    public virtual company? company { get; set; }

    [ForeignKey("education_id")]
    [InverseProperty("Vacancies")]
    public virtual Education education { get; set; } = null!;
}
