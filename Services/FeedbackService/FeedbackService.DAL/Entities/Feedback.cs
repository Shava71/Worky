using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.DAL.Entities;

public partial class Feedback
{
    public Guid Id { get; set; }
    public Guid resumeId { get; set; }
    public Guid vacancyId { get; set; }
    public FeedbackStatus status { get; set; } = FeedbackStatus.InProgress;
    public DateOnly income_date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public Resume resume { get; set; } = null!;
    
    public  Vacancy vacancy { get; set; } = null!;
}

public enum FeedbackStatus
{
    InProgress,
    Accepted,
    Cancelled,
}