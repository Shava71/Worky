using Elastic.Clients.Elasticsearch;

namespace SearchService.DAL.Entities;

public class VacancyDocument
{
    public Guid id { get; set; }
    public Guid companyId { get; set; }
    public string post { get; set; } = null!;
    public int minSalary { get; set; }
    public int? maxSalary { get; set; }
    public int educationId { get; set; }
    public string educationName { get; set; } = null!;
    public int? experience { get; set; }
    public string? description { get; set; }
    public DateTime incomeDate { get; set; }

    public string workFormat { get; set; } = null!;
    public string workHour { get; set; } = null!;

    public CompanyInfo company { get; set; } = null!;
    public List<Activity> activities { get; set; } = new();

    // Geo-поле для поиска по расстоянию
    public GeoLocation? location { get; set; }
}