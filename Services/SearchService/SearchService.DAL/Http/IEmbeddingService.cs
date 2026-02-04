namespace SearchService.BLL.Services.Interfaces;

public interface IEmbeddingService
{
    Task<float[]> GetEmbedding(string text, CancellationToken ct = default);
}