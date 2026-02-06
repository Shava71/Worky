using CompanyService.DAL.Entities;

namespace CompanyService.DAL.DTO;

public class DealDto
{
    public Guid id { get; set; }
    public int tariff_id { get; set; }
    public Tarrif tariff { get; set; }
    public string? company_id { get; set; }
    public DateOnly date_start { get; set; }
    public DateOnly date_end { get; set; }
    public int sum { get; set; }
}