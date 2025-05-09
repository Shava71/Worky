using System.ComponentModel.DataAnnotations;

namespace Worky.DTO;

public class ActivityDtos
{    
    [Required]
    public ulong id { get; set; }
    [Required]
    public string direction { get; set; } = null!;
    [Required]
    public string type { get; set; } = null!;
    [Required]
    public ulong filter_id { get; set; }
}