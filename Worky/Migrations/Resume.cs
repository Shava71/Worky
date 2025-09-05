using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Worky.Migrations;

[Table("Resume")]
[Index("income_date", Name = "income_date")]
[Index("education_id", Name = "resume_ibfk_2")]
[Index("worker_id", Name = "worker_ibfk_1")]
public partial class Resume
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong id { get; set; }

    [StringLength(450)] public string? worker_id { get; set; }

    [Column(TypeName = "text")] public string? skill { get; set; }

    [Column(TypeName = "text")] public string? city { get; set; }

    [Column(TypeName = "smallint(6)")] public short? experience { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong? education_id { get; set; }

    [Column(TypeName = "datetime")] public DateTime income_date { get; set; }

    [Column(TypeName = "int(11)")] public int? wantedSalary { get; set; }

    [StringLength(100)] public string post { get; set; } = null!;

    [InverseProperty("resume")]
    [JsonIgnore]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("resume")]
    public virtual ICollection<Resume_filter> Resume_filters { get; set; } = new List<Resume_filter>();

    [ForeignKey("education_id")]
    [InverseProperty("Resumes")]
    public virtual Education? education { get; set; }

    [ForeignKey("worker_id")]
    [InverseProperty("Resumes")]
    [JsonIgnore]
    public virtual Worker? worker { get; set; }
}