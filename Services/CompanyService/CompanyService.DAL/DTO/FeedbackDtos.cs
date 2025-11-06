
using CompanyService.DAL.Models;

namespace CompanyService.DAL.DTO;

public class FeedbackDtos
{
    public Guid id { get; set; }

    public Guid resume_id { get; set; }

    public Guid vacancy_id { get; set; }

    public FeedbackStatus status { get; set; }
}