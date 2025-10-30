namespace WorkerService.DAL.Contracts;

public class UpdateResume
{
    public Guid id { get; set; }
    public string? skill { get; set; }
    public string? city { get; set; }
    public int? experience { get; set; }
    public int? education_id { get; set; }
    public int? wantedSalary { get; set; }
    public string? post { get; set; }
}