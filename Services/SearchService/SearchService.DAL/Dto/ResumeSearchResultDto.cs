using SearchService.DAL.Entities;

namespace SearchService.DAL.Dto;

public class ResumeSearchResultDto
{
    public ResumeDocument Document { get; set; } = null!;
    public double Score { get; set; }
}