using System.ComponentModel.DataAnnotations;

namespace Worky.DTO;

public class WorkerDtos
{
    [Required]
    public string id { get; set; } = null!;
    [Required]
    public string second_name { get; set; } = null!;
    [Required]
    public string first_name { get; set; } = null!;
    [Required]
    public string surname { get; set; }
    [Required] public short age { get; set; }
    [Required]
    public byte[]? image { get; set; }
}