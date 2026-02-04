namespace SearchService.DAL.Embenddings;

public class EmbeddingOptions
{
    public string BaseUrl { get; init; } = default!;
    public string Model { get; init; } = default!;
    public int TimeoutSeconds { get; init; } = 10;
}