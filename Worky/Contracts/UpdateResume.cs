namespace Worky.Contracts;

public class UpdateResume
{
    public ulong id { get; set; }
    public string? skill { get; set; }
    public string? city { get; set; }
    public short? experience { get; set; }
    public ulong? education_id { get; set; }
    public int? wantedSalary { get; set; }
    public string post { get; set; }
}