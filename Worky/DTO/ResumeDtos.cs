using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Worky.DTO;

public class ResumeDtos
{
    [Required]
    public ulong id { get; set; }
    [Required]
    public string? worker_id { get; set; }
    [Required]
    public string? skill { get; set; }
    [Required]
    public string? city { get; set; }
    [Required]
    public short? experience { get; set; }
    [Required]
    public ulong? education_id { get; set; }
    [Required]
    public DateTime income_date { get; set; }
    [Required]
    public int? wantedSalary { get; set; }
    [Required]
    public string? post { get; set; }
    [Required]
    public List<ActivityDtos>? activities { get; set; }
    [Required]
    public WorkerDtos? worker { get; set; }
}


