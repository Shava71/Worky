namespace SearchService.Contract;

public class ClickRequest
{
    public Guid SessionId { get; set; }
    public Guid DocumentId { get; set; }
    public int DwellTimeMs { get; set; }
}