using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkerService.DAL.HttpClients.Clients;


public class TypeOfActivityResponse
{
 
    public int id { get; set; }
    [Required]
    public required string direction { get; set; }
    [Required]
    public required string type { get; set; } 
    
}