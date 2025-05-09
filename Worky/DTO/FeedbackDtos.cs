using Worky.Migrations;

namespace Worky.DTO;

public class FeedbackDtos
{
    public ulong id { get; set; }

    public ulong resume_id { get; set; }

    public ulong vacancy_id { get; set; }
    
    public FeedbackStatus status { get; set; }
}