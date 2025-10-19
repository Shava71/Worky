using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkerService.DAL.Entities;

[Table("Worker")]
public partial class Worker
{
    public Guid UserId { get; set; } = Guid.NewGuid();

    public string second_name { get; set; } = null!;

    public string first_name { get; set; } = null!;

    public string surname { get; set; } = null!;

    public DateOnly birthday { get; set; }

    public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}