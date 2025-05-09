namespace Worky.Contracts;

public class UpdateVacancy
{
    public ulong Id { get; set; }
    public string Post { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public int EducationId { get; set; }
    public string Experience { get; set; }  
    public string Description { get; set; }
}