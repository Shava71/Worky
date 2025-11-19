
using SearchService.DAL.HttpClients.Clients;

namespace SearchService.DAL.Events;

public record VacancyFilterAddEvent(Guid vacancy_id, List<TypeOfActivityResponse> activities);