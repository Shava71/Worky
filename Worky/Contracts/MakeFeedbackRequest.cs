namespace Worky.Contracts;

public record MakeFeedbackRequest(
    ulong vacancy_id,
    ulong resume_id
);