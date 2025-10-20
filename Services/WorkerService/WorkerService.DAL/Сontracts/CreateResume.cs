namespace WorkerService.DAL.Contracts;

public record CreateResume(
    string skill,
    string? city,
    int? experience,
    int? education_id,
    int? wantedSalary,
    string post
);