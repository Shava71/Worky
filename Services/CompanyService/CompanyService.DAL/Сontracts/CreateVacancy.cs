using CompanyService.DAL.Entities;
using CompanyService.DAL.Entities.TypeConfiguration;

namespace CompanyService.DAL.Contracts;

public record
    CreateVacancy(
        string post,
        int min_salary,
        int? max_salary,
        ulong? education_id,
        short? experience,
        string? description,
        WorkFormat? work_format,
        WorkHour? work_hour
    );