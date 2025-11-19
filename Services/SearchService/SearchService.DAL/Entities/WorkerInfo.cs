namespace SearchService.DAL.Entities;

public class WorkerInfo
{
    public string id { get; set; } = null!;
    public string secondName { get; set; } = null!;
    public string firstName { get; set; } = null!;
    public string surname { get; set; } = null!;
    public DateOnly birthday { get; set; }
    public int? age { get; set; }
    public string? email { get; set; }
    public string? phone { get; set; }
}