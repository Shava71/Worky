using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkerService.DAL.Entities;


public partial class Resume_filter
{
    public Guid filter_id { get; set; }

    public Guid resume_id { get; set; }

    public int typeOfActivity_id { get; set; }
    
    public virtual Resume resume { get; set; } = null!;
    
}
