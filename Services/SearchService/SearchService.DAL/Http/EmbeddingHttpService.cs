using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SearchService.BLL.Services.Interfaces;
using SearchService.DAL.Embenddings;
using SearchService.DAL.Utils;

namespace SearchService.BLL.Services.Implementations;

public class EmbeddingHttpService : IEmbeddingService
{
    private readonly HttpClient _http;
    private readonly EmbeddingOptions _options;

    public EmbeddingHttpService(
        HttpClient http,
        IOptions<EmbeddingOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task<float[]> GetEmbedding(string text, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text for embedding cannot be empty.");

        text = SearchTextNormalizer.Normalize(text);

        var payload = new
        {
            model = _options.Model,
            input = text
        };

        using var response = await _http.PostAsJsonAsync(
            "/v1/embeddings",
            payload,
            ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<EmbeddingResponse>(cancellationToken: ct);

        if (result?.Data.Count == 0)
            throw new InvalidOperationException("Embedding service returned empty result.");

        return result!.Data[0].Embedding;
    }
}
