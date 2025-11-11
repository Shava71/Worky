namespace CompanyService.DAL.Events;

public record ResumeFilterDeleteEvent(Guid vacancy_id, int activity_id);