namespace FeedbackService.DAL.Contracts;

public class MakeFeedbackRequest
{
    public Guid vacancy_id { get; set; }
    public Guid resume_id { get; set; }
}