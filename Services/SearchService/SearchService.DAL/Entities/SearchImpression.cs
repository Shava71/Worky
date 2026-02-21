namespace SearchService.DAL.Entities;

public class SearchImpression
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public SearchSession Session { get; set; } = null!;
    public Guid DocumentId { get; set; }
    public string DocumentType { get; set; } = null!;
    public int Position { get; set; }
    public bool Clicked { get; set; }
    public int? DwellTimeMs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}