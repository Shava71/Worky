using System.ComponentModel.DataAnnotations;

namespace SearchService.Contract;

public record GetResumesRequest(
    Guid? id,
    int? min_experience,
    int? max_experience,
    [Range(1, 8)] int? education,
    string? city,
    // string? latitude,
    // string? longitude,
    DateTime? income_date,
    int? min_wantedSalary,
    int? max_wantedSalary,
    [MinLength(3)] [MaxLength(4)] string? Order,
    string? SortItem,
    string? type,
    List<string>? direction,
    
    string? AISearch, // это умный поиск по ключевым словам, который может содержаться в любом поле документа (например, "C# developer 5000"
    int Page,
    int PageSize,
    Guid? UserId
);