using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Worky.Migrations;

[Table("Worker")]
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

    [Column(TypeName = "date")]
    public DateOnly birthday { get; set; }
    
    [JsonIgnore]
    [InverseProperty("worker")]
    public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}
