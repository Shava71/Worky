namespace FeedbackService.DAL.Entities;

public class Resume
{
    public Guid resumeId { get; set; }
    public Guid workerId { get; set; }
    
    public ICollection<Feedback> feedbacks { get; set; }
}