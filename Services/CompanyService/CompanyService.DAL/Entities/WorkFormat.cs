
using Microsoft.OpenApi.Attributes;

namespace CompanyService.DAL.Entities;

public enum WorkFormat
{
    [Display(name: "Офис")] Office = 1,
    [Display(name: "Гибрид")] Hybrid = 2,
    [Display(name: "Удалённый")] Remote = 3
}