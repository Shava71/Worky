namespace SearchService.DAL.Entities;

public class CompanyInfo
{
    public Guid id { get; set; }
    public string name { get; set; } = null!;
    public string? email { get; set; }
    public string? phoneNumber { get; set; }
    public string? website { get; set; }
}