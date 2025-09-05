using System.ComponentModel.DataAnnotations;

namespace Worky.Contracts;

public record GetResumesRequest(
    ulong? id,
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
    List<string>? direction
);