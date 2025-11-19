using System.ComponentModel.DataAnnotations;
using SearchService.DAL.Clients;

namespace SearchService.DAL.DTO;

public class WorkerProfileDto
{
    [Required] public UserResponse UserResponse { get; set; }
    [Required] public WorkerDtos worker { get; set; } 
}