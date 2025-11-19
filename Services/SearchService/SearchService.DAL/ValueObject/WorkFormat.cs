using Microsoft.OpenApi;
using Microsoft.OpenApi.Attributes;

namespace SearchService.DAL.Entities;

public enum WorkFormat
{
    [Display(name: "Офис")] Office = 1,
    [Display(name: "Гибрид")] Hybrid = 2,
    [Display(name: "Удалённый")] Remote = 3
}