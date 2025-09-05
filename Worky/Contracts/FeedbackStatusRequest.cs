using Worky.Migrations;

namespace Worky.Contracts;

public record FeedbackStatusRequest(
    ulong feedback_id,
    string status
);