using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Education")]
public partial class Education
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong id { get; set; }

    [StringLength(50)]
    public string name { get; set; } = null!;

    [InverseProperty("education")]
    public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();

    [InverseProperty("education")]
    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
