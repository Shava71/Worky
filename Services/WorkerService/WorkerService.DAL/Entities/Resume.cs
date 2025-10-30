using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkerService.DAL.Entities;

public partial class Resume
{
    public Guid id { get; set; }

    public Guid? worker_id { get; set; }

    public string? skill { get; set; }

    public string? city { get; set; }

    public int? experience { get; set; }

    public int? education_id { get; set; }

    public DateTime income_date { get; set; } = DateTime.UtcNow;

    public int? wantedSalary { get; set; }

    public string post { get; set; } = null!;
    
    public virtual ICollection<Resume_filter> resume_filters { get; set; } = new List<Resume_filter>();

    public virtual Education? education { get; set; }
    
    public virtual Worker? worker { get; set; }
}