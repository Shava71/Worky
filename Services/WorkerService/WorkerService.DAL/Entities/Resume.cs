using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkerService.DAL.Entities;

[Table("Resume")]
[Index("income_date", Name = "income_date")]
[Index("education_id", Name = "resume_ibfk_2")]
[Index("worker_id", Name = "worker_ibfk_1")]
public partial class Resume
{
    public Guid id { get; set; }

    public string? worker_id { get; set; }

    public string? skill { get; set; }

    public string? city { get; set; }

    public short? experience { get; set; }

    public ulong? education_id { get; set; }

    public DateTime income_date { get; set; } = DateTime.UtcNow;

    public int? wantedSalary { get; set; }

    public string post { get; set; } = null!;
    
    public virtual ICollection<Resume_filter> resume_filters { get; set; } = new List<Resume_filter>();

    public virtual Education? education { get; set; }
    
    public virtual Worker? worker { get; set; }
}