
namespace WorkerService.DAL.Contracts;

public record FeedbackStatusRequest(
    ulong feedback_id,
    string status
);