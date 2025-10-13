using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Events;

public class UserCompanyCreatedEvent
{
    [Required]
    public string UserId { get; set; }
    [Required]
    public string name { get; set; }
    [Required]
    public string latitude { get; set; }
    [Required]
    public string longitude { get; set; }
    [Required]
    public string email_info { get; set; }
    [Required]
    public string phone_info { get; set; }
    [Required]
    public string website { get; set; }
}