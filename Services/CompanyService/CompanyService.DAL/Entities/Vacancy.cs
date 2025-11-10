using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompanyService.DAL.Entities.TypeConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.DAL.Entities;

public partial class Vacancy
{
    public Guid id { get; set; }

    public Guid company_id { get; set; }

    public string post { get; set; } = null!;

    public int min_salary { get; set; }

    public int education_id { get; set; }

    public int? experience { get; set; }

    public string? description { get; set; }

    public DateTime income_date { get; set; }
    
    public int? max_salary { get; set; }
    
    public WorkFormat? work_format { get; set; }
    
    public WorkHour? work_hour { get; set; }

    public virtual ICollection<Vacancy_filter> Vacancy_filters { get; set; } = new List<Vacancy_filter>();

    public virtual Company? company { get; set; }
    
    public virtual Education education { get; set; } = null!;
}