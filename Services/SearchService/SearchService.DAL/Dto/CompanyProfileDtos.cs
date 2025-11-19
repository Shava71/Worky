using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SearchService.DAL.Clients;

namespace SearchService.DAL.DTO;

public class CompanyProfileDtos
{
    [Required] public UserResponse user { get; set; }
    [Required] public CompanyDto company { get; set; }
    [Required] public List<DealDto> deals { get; set; }
}