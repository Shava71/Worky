using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Events;

public class UserWorkerCreatedEvent
{
    [Required]
    public string UserId { get; set; }
    [Required]
    public string second_name { get; set; } = null!;
    [Required]
    public string first_name { get; set; } = null!;
    [Required]
    public string surname { get; set; } = null!;
    [Required]
    public DateOnly birthday { get; set; }
}