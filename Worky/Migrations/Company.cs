using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Worky.Migrations;

[Table("company")]
[Index("email", Name = "email", IsUnique = true)]
public partial class Company
{
    [Key] [StringLength(450)] public string id { get; set; } = null!;

    [Column(TypeName = "text")] public string? name { get; set; }

    [Column(TypeName = "text")] public string email { get; set; } = null!;

    [StringLength(20)] public string? phoneNumber { get; set; }

    public Point office_coord { get; set; } = null!;

    public string? website { get; set; }

    [InverseProperty("company")] public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();

    [InverseProperty("company")]
    public virtual ICollection<RecruitManager> RecruitManagers { get; set; } = new List<RecruitManager>();

    [InverseProperty("company")] public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}