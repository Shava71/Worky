using CompanyService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkerService.DAL.Entities;

public class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.HasKey(r => r.id);

        builder.Property(r => r.post)
            .IsRequired();
        
        builder.Property(r => r.experience)
            .IsRequired();
        
        builder.Property(r => r.income_date)
            .IsRequired();
        
        builder.Property(r => r.min_salary)
            .IsRequired();
     
        builder.HasOne(r => r.company)
            .WithMany(w => w.Vacancies)
            .HasForeignKey(r => r.company_id)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(r => r.education)
            .WithMany(e => e.vacancies)
            .HasForeignKey(r => r.education_id)
            .OnDelete(DeleteBehavior.SetNull);
    }
}