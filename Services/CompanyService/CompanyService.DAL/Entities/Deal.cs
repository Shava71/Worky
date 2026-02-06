using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


namespace CompanyService.DAL.Entities;

public partial class Deal
{
    public Guid id { get; set; }

    public int tariff_id { get; set; }

    public Guid? company_id { get; set; }
    
    public DateOnly date_start { get; set; }

    public DateOnly date_end { get; set; }

   public int sum { get; set; }

    [JsonIgnore]
    public virtual Company? company { get; set; }

    [JsonIgnore]
    public virtual Tarrif tariff { get; set; } = null!;
}