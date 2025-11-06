namespace CompanyService.DAL.Entities.TypeConfiguration;

public enum WorkHour
{
    FullTime = 1, // Полная занятость (40 часов в неделю)
    PartTime = 2, // Частичная занятость (20–30 часов)
    Flexible = 3, // Гибкий график
    ShiftWork = 4, // Сменный график
    ProjectBased = 5, // Проектная / временная работа
    Internship = 6 // Стажировка / обучение
}