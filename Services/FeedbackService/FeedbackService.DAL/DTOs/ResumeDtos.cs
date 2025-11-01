using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FeedbackService.DAL.DTO;

public class ResumeDtos
{
    [Required] public Guid id { get; set; }
    [Required] public string? worker_id { get; set; }
}