namespace SearchService.DAL.Events;

public record ResumeFilterDeleteEvent(Guid resume_id, int activity_id);