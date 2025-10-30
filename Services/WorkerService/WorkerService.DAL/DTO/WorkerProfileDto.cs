using System.ComponentModel.DataAnnotations;
using WorkerService.DAL.Clients;

namespace WorkerService.DAL.DTO;

public class WorkerProfileDto
{
    [Required] public UserResponse UserResponse { get; set; }
    [Required] public WorkerDtos worker { get; set; } 
}