using System.ComponentModel.DataAnnotations;

namespace WorkerService.BLL.Events;

public class UserWorkerCreateFailedEvent
{
    [Required]
    public string UserId { get; set; } = null!;
    [Required]
    public string Reason { get; set; } = null!;
}