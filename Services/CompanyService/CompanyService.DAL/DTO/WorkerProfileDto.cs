using System.ComponentModel.DataAnnotations;
using CompanyService.DAL.Clients;
using WorkerService.DAL.DTO;

namespace CompanyService.DAL.DTO;

public class WorkerProfileDto
{
    [Required] public UserResponse UserResponse { get; set; }
    [Required] public WorkerDtos worker { get; set; } 
}