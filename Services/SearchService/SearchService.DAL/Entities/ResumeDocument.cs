namespace SearchService.DAL.Entities;

public class ResumeDocument
{
    public Guid id { get; set; }
    public string workerId { get; set; } = null!;
    public string skill { get; set; } = null!;
    public string city { get; set; } = null!;
    public int experience { get; set; }
    public int educationId { get; set; }
    public string educationName { get; set; } = null!;
    public DateTime incomeDate { get; set; }
    public int? wantedSalary { get; set; }
    public string post { get; set; } = null!;

    // Вложенные объекты
    public WorkerInfo worker { get; set; } = null!;
    public List<Activity> activities { get; set; } = new();

    // Для сортировки и фильтрации
    public string WorkerFullName => $"{worker.secondName} {worker.firstName} {worker.surname}".Trim();
}