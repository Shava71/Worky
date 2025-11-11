
using WorkerService.DAL.HttpClients.Clients;

namespace CompanyService.DAL.Events;

public record ResumeFilterAddEvent(Guid resume_id, List<TypeOfActivityResponse> activities);