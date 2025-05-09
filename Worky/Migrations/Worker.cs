using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Worker")]
[Index("email", Name = "email", IsUnique = true)]
public partial class Worker
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

    [StringLength(20)]
    public string? phoneNumber { get; set; }

    [Column(TypeName = "text")]
    public string email { get; set; } = null!;

    [InverseProperty("worker")]
    public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}
