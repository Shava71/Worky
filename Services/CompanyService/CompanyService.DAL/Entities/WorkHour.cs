using Microsoft.OpenApi.Attributes;

namespace CompanyService.DAL.Entities.TypeConfiguration;

public enum WorkHour
{
    [Display(name: "Полная (40 часов)")] FullTime = 1, // Полная занятость (40 часов в неделю)
    [Display(name: "Частичная (20-30 часов)")] PartTime = 2, // Частичная занятость (20–30 часов)
    [Display(name: "Гибкий график")] Flexible = 3, // Гибкий график
    [Display(name: "Сменный график")] ShiftWork = 4, // Сменный график
    [Display(name: "Проектная работа")] ProjectBased = 5, // Проектная / временная работа
    [Display(name: "Стажировка")] Internship = 6 // Стажировка / обучение
}