namespace WorkerService.DAL.Contracts;

public record MakeFeedbackRequest(
    Guid vacancy_id,
    Guid resume_id
);