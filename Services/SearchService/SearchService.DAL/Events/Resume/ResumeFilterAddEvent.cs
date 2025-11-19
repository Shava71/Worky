
using SearchService.DAL.HttpClients.Clients;

namespace SearchService.DAL.Events;

public record ResumeFilterAddEvent(Guid resume_id, List<TypeOfActivityResponse> activities);