using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Resume_filter")]
[Index("resume_id", Name = "resume_id")]
[Index("typeOfActivity_id", Name = "typeOfActivity_id")]
public partial class Resume_filter
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong filter_id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong resume_id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong typeOfActivity_id { get; set; }

    [ForeignKey("resume_id")]
    [InverseProperty("Resume_filters")]
    public virtual Resume resume { get; set; } = null!;

    [ForeignKey("typeOfActivity_id")]
    [InverseProperty("Resume_filters")]
    public virtual TypeOfActivity typeOfActivity { get; set; } = null!;
}
