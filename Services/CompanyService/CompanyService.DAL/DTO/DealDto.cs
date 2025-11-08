namespace CompanyService.DAL.DTO;

public class DealDto
{
    public ulong id { get; set; }
    public ulong tariff_id { get; set; }
    public string? company_id { get; set; }
    public bool status { get; set; }
    public DateOnly date_start { get; set; }
    public DateOnly date_end { get; set; }
    public int sum { get; set; }
}