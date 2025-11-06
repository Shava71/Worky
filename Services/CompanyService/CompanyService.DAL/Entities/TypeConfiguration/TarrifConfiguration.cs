using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyService.DAL.Entities.TypeConfiguration;

public class TarrifConfiguration : IEntityTypeConfiguration<Tarrif>
{
    public void Configure(EntityTypeBuilder<Tarrif> builder)
    {
        builder.HasKey(t => t.id);

        builder.Property(t => t.name).IsRequired();
        builder.Property(t => t.price).IsRequired();
        builder.Property(t => t.description).IsRequired();
        builder.Property(t => t.vacancy_count).IsRequired();

        builder.HasData(
            new Tarrif
            {
                id = 1,
                name = "Базовый",
                price = 990,
                description = "Размещение 1 вакансии без продвижения.",
                vacancy_count = 1
            },
            new Tarrif
            {
                id = 2,
                name = "Стандартный",
                price = 2990,
                description = "Размещение 3 вакансий, выделение цветом в списке.",
                vacancy_count = 3
            },
            new Tarrif
            {
                id = 3,
                name = "Продвинутый",
                price = 5990,
                description = "Размещение 5 вакансий, поднятие в поиске раз в 3 дня.",
                vacancy_count = 5
            },
            new Tarrif
            {
                id = 4,
                name = "Премиум",
                price = 9990,
                description = "Размещение 10 вакансий, ежедневное поднятие.",
                vacancy_count = 10
            }
        );
    }
}