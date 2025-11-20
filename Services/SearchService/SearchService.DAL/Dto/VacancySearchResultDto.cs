using SearchService.DAL.Entities;

namespace SearchService.DAL.Dto;

public class VacancySearchResultDto
{
    public VacancyDocument Document { get; set; } = null!;
    public double Score { get; set; }
}