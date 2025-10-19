using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkerService.DAL.Entities;

public partial class Education
{
    public int id { get; set; }

    public string name { get; set; } = null!;
}