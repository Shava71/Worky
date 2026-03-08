using System.Text.Json.Serialization;
using Elastic.Clients.Elasticsearch;

namespace SearchService.DAL.Entities;

public class VacancyDocument
{
    [JsonPropertyName("id")] public Guid id { get; set; }
    [JsonPropertyName("companyId")] public Guid companyId { get; set; }
    [JsonPropertyName("post")] public string post { get; set; } = null!;
    [JsonPropertyName("minSalary")] public int minSalary { get; set; }
    [JsonPropertyName("maxSalary")] public int? maxSalary { get; set; }
    [JsonPropertyName("educationId")] public int educationId { get; set; }
    [JsonPropertyName("educationName")] public string educationName { get; set; } = null!;
    [JsonPropertyName("experience")] public int? experience { get; set; }
    [JsonPropertyName("description")] public string? description { get; set; }
    [JsonPropertyName("incomeDate")] public DateTime incomeDate { get; set; }
    [JsonPropertyName("workFormat")] public string workFormat { get; set; } = null!;
    [JsonPropertyName("workHour")] public string workHour { get; set; } = null!;
    [JsonPropertyName("company")] public CompanyInfo company { get; set; } = null!;
    [JsonPropertyName("activities")] public List<Activity> activities { get; set; } = new();
    // [JsonPropertyName("location"), JsonConverter(typeof(GeoLocationConverter))] public GeoLocation? location { get; set; }
    [JsonPropertyName("location")] public LatLonGeoLocation? location { get; set; }
    [JsonPropertyName("vector")] public float[]? vector { get; set; }
}