namespace CompanyService.DAL.DTO;

public class VacancyRow
{
    public Guid id { get; set; }
    public Guid company_id { get; set; }
    public string post { get; set; }
    public int min_salary { get; set; }
    public int? max_salary { get; set; }
    public int education_id { get; set; }
    public string education_name { get; set; }
    public int experience { get; set; }
    public string description { get; set; }
    public DateTime income_date { get; set; }
    public int? work_format { get; set; }
    public int? work_hour { get; set; }

    // из vacancy_filter
    public Guid? FilterId { get; set; }
    public int? TypeOfActivityId { get; set; }
}