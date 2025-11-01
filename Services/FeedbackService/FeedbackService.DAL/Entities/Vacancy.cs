namespace FeedbackService.DAL.Entities;

public class Vacancy
{
    public Guid vacancyId { get; set; }
    public Guid companyId { get; set; }
    
    public ICollection<Feedback> feedbacks { get; set; }

}