using System.Text.Json.Serialization;

namespace SearchService.DAL.Embenddings;

public class EmbeddingResponse
{
    [JsonPropertyName("data")]
    public List<EmbeddingItem> Data { get; init; } = new();
}