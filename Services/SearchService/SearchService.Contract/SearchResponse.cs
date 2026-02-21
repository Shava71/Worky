namespace SearchService.Contract;

public class SearchResponse<T>
{
    public long Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public Guid SessionId { get; set; }

    public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();
}