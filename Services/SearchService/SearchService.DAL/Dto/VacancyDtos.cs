using System.ComponentModel.DataAnnotations;
using Elastic.Clients.Elasticsearch;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;
using SearchService.DAL.Entities.TypeConfiguration;
using SearchService.DAL.HttpClients.Clients;


namespace SeachService.DAL.DTO;

public class VacancyDtos
{
    [Required] public Guid id { get; set; }
    [Required] public Guid? company_id { get; set; }
    [Required] public string post { get; set; } = null!;
    [Required] public int min_salary { get; set; }
    [Required] public int education_id { get; set; }
    [Required] public string education_name { get; set; }
    [Required] public int? experience { get; set; }
    [Required] public string? description { get; set; }
    [Required] public DateTime income_date { get; set; }
    [Required] public int? max_salary { get; set; }
    [Required] public WorkFormat? work_format { get; set; }
    [Required] public string work_format_name  => work_format?.GetDisplayName();
    [Required] public WorkHour? work_hour { get; set; }
    [Required] public string work_hour_name  => work_hour?.GetDisplayName();
    [Required] public List<TypeOfActivityResponse>? activities { get; set; }
    [Required] public CompanyDto? company { get; set; }
    
    
    //  geo_point поле в elasticsearch
    public LatLonGeoLocation? Location => 
        company != null && 
        double.TryParse(company.latitude, out var lat) && 
        double.TryParse(company.longitude, out var lon)
            ? new LatLonGeoLocation { Lat = lat, Lon = lon }
            : null;
    
}