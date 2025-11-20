using System.Text.Json.Serialization;

namespace SearchService.DAL.Entities;

public class Activity
{
    [JsonPropertyName("id")] public int id { get; set; }
    [JsonPropertyName("direction")] public string direction { get; set; } = null!;
    [JsonPropertyName("type")] public string type { get; set; } = null!;
}