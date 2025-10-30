using WorkerService.DAL.DTO;

namespace WorkerService.BLL.Events;

public record ResumeUpdatedEvent(ResumeDtos Resume) : ResumeCreatedEvent(Resume);