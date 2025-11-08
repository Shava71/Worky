
using CompanyService.DAL.DTO;

namespace CompanyService.DAL.Events;

public record VacancyUpdatedEvent(VacancyDtos Vacancy) : VacancyCreatedEvent(Vacancy);