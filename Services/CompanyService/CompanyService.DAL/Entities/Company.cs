using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.DAL.Entities;

public partial class Company
{
    public Guid UserId { get; set; }

    public string name { get; set; }

    public string email { get; set; } = null!;

    public string? phoneNumber { get; set; }

    public string latitude { get; set; }
    public string longitude { get; set; }

    public string? website { get; set; }

    public virtual ICollection<Deal>? Deals { get; set; } 

    public virtual ICollection<Vacancy>? Vacancies { get; set; } = new List<Vacancy>();
}