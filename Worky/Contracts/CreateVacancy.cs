namespace Worky.Contracts;

public record CreateVacancy(
    string post,
    int min_salary,
    int? max_salary,
    ulong? education_id,
    short? experience,
    string? description
    );