using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Worky.Migrations;

[Table("Feedback")]
[Index("resume_id", Name = "resume_id")]
[Index("vacancy_id", Name = "vacancy_id")]
public partial class Feedback
{
    [Key]
    [Column(TypeName = "bigint(20) unsigned")]
    public ulong id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong resume_id { get; set; }

    [Column(TypeName = "bigint(20) unsigned")]
    public ulong vacancy_id { get; set; }
    
    [Column(TypeName = "enum")]
    public FeedbackStatus status { get; set; } = FeedbackStatus.InProgress;

    [ForeignKey("resume_id")]
    [InverseProperty("Feedbacks")]
    public virtual Resume resume { get; set; } = null!;

    [ForeignKey("vacancy_id")]
    [InverseProperty("Feedbacks")]
    public virtual Vacancy vacancy { get; set; } = null!;


    
}

public enum FeedbackStatus
{
    InProgress,
    Accepted,
    Cancelled,
}
