using System.Text.Json.Serialization;

namespace SearchService.DAL.Embenddings;

public class EmbeddingItem
{
    [JsonPropertyName("embedding")]
    public float[] Embedding { get; init; } = Array.Empty<float>();
}