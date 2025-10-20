using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace WorkerService.DAL.DTO;

public class ResumeDtos
{
    [Required] public Guid id { get; set; }
    [Required] public string? worker_id { get; set; }
    [Required] public string? skill { get; set; }

    [Required]
    [RegularExpression("^[^0-9]*$", ErrorMessage = "Название города не должно содержать цифры")]
    public string? city { get; set; }

    [Required] public int? experience { get; set; }
    [Required] public int? education_id { get; set; }
    [Required] public DateTime income_date { get; set; }
    [Required] public int? wantedSalary { get; set; }
    [Required] public string? post { get; set; }
    [Required] public List<ActivityDtos>? activities { get; set; }
    [Required] public WorkerDtos? worker { get; set; }
}