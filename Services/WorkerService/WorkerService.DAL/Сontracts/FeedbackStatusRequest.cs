
namespace WorkerService.DAL.Contracts;

public record FeedbackStatusRequest(
    int feedback_id,
    string status
);