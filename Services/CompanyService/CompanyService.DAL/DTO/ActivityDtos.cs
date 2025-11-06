using System.ComponentModel.DataAnnotations;

namespace CompanyService.DAL.DTO;

public class ActivityDtos
{
    [Required] public int id { get; set; }
    [Required] public string direction { get; set; } = null!;
    [Required] public string type { get; set; } = null!;
    [Required] public int filter_id { get; set; }
}