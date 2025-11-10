using FeedbackService.DAL.DTO;

namespace FeedbackService.BLL.Events;

public record VacancyCreatedEvent(VacancyDto Vacancy);
