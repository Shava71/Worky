namespace WorkerService.DAL.DTO;

public class CompanyDto
{
    public string id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string? phoneNumber { get; set; }
    public string? website { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
}