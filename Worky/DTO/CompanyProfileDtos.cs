using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Worky.Migrations;

namespace Worky.DTO;

public class CompanyProfileDtos
{
    [Required]
    public Users user { get; set; }
    [Required]
    public CompanyDto company { get; set; }
    [Required]

    public List<DealDto> deals { get; set; }
}

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