using System.ComponentModel.DataAnnotations;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Entities.TypeConfiguration;

namespace CompanyService.DAL.Contracts;

public record GetVacanciesRequest(
    Guid? id,
    string? search,
    int? min_experience,
    int? max_experience,
    [Range(1, 8)] int? education,
    string? city,
    DateTime? income_date,
    int? min_wantedSalary,
    int? max_wantedSalary,
    [MinLength(3)] [MaxLength(4)] string? Order,
    string? SortItem,
    string? type,
    List<string>? direction,
    WorkFormat? workFormat,
    WorkHour? workHour
);