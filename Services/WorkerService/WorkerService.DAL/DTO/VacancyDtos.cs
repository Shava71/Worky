using System.ComponentModel.DataAnnotations;


namespace WorkerService.DAL.DTO;

public class VacancyDtos
{
    [Required] public ulong id { get; set; }
    [Required] public string? company_id { get; set; }
    [Required] public string post { get; set; } = null!;
    [Required] public int min_salary { get; set; }
    [Required] public ulong education_id { get; set; }
    [Required] public short? experience { get; set; }
    [Required] public string? description { get; set; }
    [Required] public DateTime income_date { get; set; }
    [Required] public int? max_salary { get; set; }
    [Required] public List<ActivityDtos>? activities { get; set; }
    [Required] public CompanyDto? company { get; set; }
}