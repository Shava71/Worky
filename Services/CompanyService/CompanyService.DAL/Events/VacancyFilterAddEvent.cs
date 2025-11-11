using CompanyService.DAL.HttpClients.Clients;

namespace CompanyService.DAL.Events;

public record VacancyFilterAddEvent(Guid vacancy_id, List<TypeOfActivityResponse> activities);