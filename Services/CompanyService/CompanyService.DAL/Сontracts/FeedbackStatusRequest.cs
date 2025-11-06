
namespace CompanyService.DAL.Contracts;

public record FeedbackStatusRequest(
    int feedback_id,
    string status
);