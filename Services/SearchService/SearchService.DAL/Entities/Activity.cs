namespace SearchService.DAL.Entities;

public class Activity
{
    public int id { get; set; }
    public string direction { get; set; } = null!;
    public string type { get; set; } = null!;
}