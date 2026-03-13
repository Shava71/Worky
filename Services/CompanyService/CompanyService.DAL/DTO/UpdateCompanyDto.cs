namespace CompanyService.DAL.DTO;

public class UpdateCompanyDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string? Website { get; set; }
}