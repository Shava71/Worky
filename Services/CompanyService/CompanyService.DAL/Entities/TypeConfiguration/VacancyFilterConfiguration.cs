using CompanyService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkerService.DAL.Entities;

public class VacancyFilterConfiguration : IEntityTypeConfiguration<Vacancy_filter>
{
    public void Configure(EntityTypeBuilder<Vacancy_filter> builder)
    {
        builder.HasKey(f => f.filter_id);

        builder.Property(f => f.vacancy_id)
            .IsRequired();

        builder.Property(f => f.typeOfActivity_id)
            .IsRequired();

        builder.HasIndex(f => new { f.vacancy_id, f.typeOfActivity_id })
            .IsUnique();

        builder.HasOne(f => f.vacancy)
            .WithMany(r => r.Vacancy_filters)
            .HasForeignKey(f => f.vacancy_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}