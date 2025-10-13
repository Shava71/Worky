namespace AuthService.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Topic { get; set; } = null!;   
    public string Type { get; set; } = null!;            
    public string Payload { get; set; } = null!; // JSON
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public bool Sent { get; set; } = false;
    public DateTime? SentAt { get; set; }
    
    public OutboxMessage(){}

    public OutboxMessage(string _topic, string _type, string _payload)
    {
        Topic = _topic;
        Type = _type;
        Payload = _payload;
    }

    public void MarkAsSent()
    {
        Sent = true;
        SentAt = DateTime.UtcNow;
    }
}