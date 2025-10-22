using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FilterService.Models;


public partial class TypeOfActivity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int id { get; set; }

    [Required]
    public required string direction { get; set; }
    [Required]
    public required string type { get; set; } 
    
}