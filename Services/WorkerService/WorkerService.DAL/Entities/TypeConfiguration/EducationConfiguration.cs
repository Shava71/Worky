using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkerService.DAL.Entities;

public class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.HasData(new Education[]
        {
            new Education() { id = 1, name = "Начальное общее образование" },
            new Education() { id = 2, name = "Основное общее образование" },
            new Education() { id = 3, name = "Среднее общее образование" },
            new Education() { id = 4, name = "Среднее профессиональное образование" },
            new Education() { id = 5, name = "Бакалавриат" },
            new Education() { id = 6, name = "Специалитет" },
            new Education() { id = 7, name = "Магистратура" },
            new Education() { id = 8, name = "Аспирантура" },
            new Education() { id = 9, name = "Ординатура" },
        });
    }
}