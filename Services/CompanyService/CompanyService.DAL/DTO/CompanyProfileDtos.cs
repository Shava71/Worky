using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CompanyService.DAL.Clients;

namespace CompanyService.DAL.DTO;

public class CompanyProfileDtos
{
    // [Required] public UserResponse user { get; set; }
    [Required] public CompanyDto company { get; set; }
    [Required] public List<DealDto> deals { get; set; }
}