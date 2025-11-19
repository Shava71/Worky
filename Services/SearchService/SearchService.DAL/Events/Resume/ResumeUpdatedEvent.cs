

using SearchService.DAL.DTO;

namespace SearchService.BLL.Events;

public record ResumeUpdatedEvent(ResumeDtos Resume) : ResumeCreatedEvent(Resume);