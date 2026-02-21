namespace SearchService.DAL.Entities;

public class SearchSession
{
    public Guid Id { get; set; }
    public string Query { get; set; } = null!;
    public Guid? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<SearchImpression> Impressions { get; set; } = new List<SearchImpression>();
}