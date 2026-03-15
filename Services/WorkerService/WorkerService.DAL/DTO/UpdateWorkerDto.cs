using System.ComponentModel.DataAnnotations;

namespace WorkerService.DAL.DTO;

public class UpdateWorkerDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string SecondName { get; set; } = null!;

    [Required]
    public string Surname { get; set; } = null!;

    [Required]
    public DateOnly Birthday { get; set; }
    
}