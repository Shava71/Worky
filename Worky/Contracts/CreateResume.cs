namespace Worky.Contracts;

public record CreateResume(
    string skill,
    string? city,
    short? experience,
    ulong? education_id,
    int? wantedSalary,
    string post
);