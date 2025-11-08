
using CompanyService.DAL.DTO;

namespace CompanyService.DAL.Events;

public record VacancyCreatedEvent(VacancyDtos Vacancy);
