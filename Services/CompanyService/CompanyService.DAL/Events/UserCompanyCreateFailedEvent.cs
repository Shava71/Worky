using System.ComponentModel.DataAnnotations;

namespace CompanyService.DAL.Events;

public class UserCompanyCreateFailedEvent
{
    [Required]
    public string UserId { get; set; } = null!;
    [Required]
    public string Reason { get; set; } = null!;
}