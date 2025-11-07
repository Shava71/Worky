using System.ComponentModel.DataAnnotations;
using CompanyService.DAL.DTO;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Entities.TypeConfiguration;
using CompanyService.DAL.HttpClients.Clients;


namespace CompanyService.DAL.DTO;

public class VacancyDtos
{
    [Required] public Guid id { get; set; }
    [Required] public Guid? company_id { get; set; }
    [Required] public string post { get; set; } = null!;
    [Required] public int min_salary { get; set; }
    [Required] public int education_id { get; set; }
    [Required] public int? experience { get; set; }
    [Required] public string? description { get; set; }
    [Required] public DateTime income_date { get; set; }
    [Required] public int? max_salary { get; set; }
    [Required] public WorkFormat? work_format { get; set; }
    [Required] public WorkHour? work_hour { get; set; }
    [Required] public List<TypeOfActivityResponse>? activities { get; set; }
    [Required] public CompanyDto? company { get; set; }
}