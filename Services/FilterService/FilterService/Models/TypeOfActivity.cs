using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FilterService.Models;


[Index("direction", "type", Name = "idx_direction_type")]
public partial class TypeOfActivity
{
    public int id { get; set; }

    public string direction { get; set; } = null!;

    public string type { get; set; } = null!;
    
}