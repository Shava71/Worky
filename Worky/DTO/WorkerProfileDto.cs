using System.ComponentModel.DataAnnotations;
using Worky.Migrations;

namespace Worky.DTO;

public class WorkerProfileDto
{
    [Required] public Users user { get; set; }
    [Required] public WorkerDtos worker { get; set; }
}