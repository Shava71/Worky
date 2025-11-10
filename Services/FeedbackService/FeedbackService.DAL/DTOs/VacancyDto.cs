using System.ComponentModel.DataAnnotations;

namespace FeedbackService.DAL.DTO;

public class VacancyDto
{
    [Required] public Guid id { get; set; }
    [Required] public Guid company_id { get; set; }
}