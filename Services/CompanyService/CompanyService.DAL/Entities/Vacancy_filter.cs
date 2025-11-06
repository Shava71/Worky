using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.DAL.Entities;

public partial class Vacancy_filter
{

    public Guid filter_id { get; set; }

    public Guid vacancy_id { get; set; }

    public int typeOfActivity_id { get; set; }

    public virtual Vacancy vacancy { get; set; } = null!;
}
