using System.Text.Json.Serialization;

namespace SearchService.DAL.Entities;

public class CompanyInfo
{
    [JsonPropertyName("id")] public Guid id { get; set; }
    [JsonPropertyName("name")] public string name { get; set; } = null!;
    [JsonPropertyName("email")] public string? email { get; set; }
    [JsonPropertyName("phoneNumber")] public string? phoneNumber { get; set; }
    [JsonPropertyName("website")] public string? website { get; set; }
}
