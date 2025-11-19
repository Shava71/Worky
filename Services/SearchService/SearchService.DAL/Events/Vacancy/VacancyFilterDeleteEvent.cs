namespace SearchService.DAL.Events;

public record VacancyFilterDeleteEvent(Guid vacancy_id, int activity_id);