using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.DAL.Entities;

public partial class Tarrif
{

    public int id { get; set; }
    
    public string name { get; set; } = null!;

    public int price { get; set; }

    public string? description { get; set; }
    
    public int vacancy_count { get; set; }

    public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
}