

using SeachService.DAL.DTO;

namespace SearchService.DAL.Events;

public record VacancyUpdatedEvent(VacancyDtos Vacancy) : VacancyCreatedEvent(Vacancy);